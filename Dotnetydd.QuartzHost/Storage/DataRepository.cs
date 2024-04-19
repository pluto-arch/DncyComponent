using Dotnetydd.QuartzHost.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Dotnetydd.QuartzHost.Storage;


internal static class AppSubscribe
{
    internal static readonly List<Subscription> ApplicationSubscriptions = new();

    internal static readonly List<Subscription> LoggerSubscriptions = new();
}


public class DataRepository
{
    private readonly IJobInfoStore _jobInfoStore;
    private readonly IJobLogStore _jobLogStore;
    private readonly object _lock = new();
    private readonly ILogger _logger;

    private readonly List<Subscription> _applicationSubscriptions = AppSubscribe.ApplicationSubscriptions;

    private readonly List<Subscription> _loggerSubscriptions = AppSubscribe.LoggerSubscriptions;

    public DataRepository(
        IJobInfoStore jobInfoStore,
        ILoggerFactory loggerFactory,
        IJobLogStore jobLogStore = null)
    {
        _logger = loggerFactory.CreateLogger(typeof(DataRepository));
        _jobInfoStore = jobInfoStore;
        _jobLogStore = jobLogStore??NullJobLogStore.Instrance;
    }


    public async Task<List<JobInfoModel>> GetJobsAsync()
    {
        var list = await _jobInfoStore.GetListAsync();
        return list;
    }


    public async Task AddJobAsync(JobInfoModel jobInfoModel)
    {
        await _jobInfoStore.AddAsync(jobInfoModel);
        RaiseSubscriptionChanged(_applicationSubscriptions);
    }

    /// <summary>
    /// JOB RUNNING LOG INFORMATION
    /// </summary>
    /// <param name="log"></param>
    /// <returns></returns>
    internal async Task AddJobLogAsync(JobLogModel log)
    {
        await _jobLogStore.AddAsync(log);
        RaiseSubscriptionChanged(_loggerSubscriptions,log);
    }

    /// <summary>
    /// APPLICATION LOG FROM ILogger
    /// </summary>
    /// <returns></returns>
    public void AddAppRunningLogAsync(string logFormatString)
    {
        RaiseSubscriptionChanged(_loggerSubscriptions,logFormatString);
    }


    public async Task<List<JobLogModel>> GetJobLogsAsync(string jobKey, int pageNo = 1, int count = 20)
    {
        return await _jobLogStore.GetListAsync(jobKey,pageNo,count);
    }


    public Subscription OnNewApplications(Func<SubscribeCallbackContext,Task> callback)
    {
        return AddSubscription(string.Empty, SubscriptionType.Read, callback, _applicationSubscriptions);
    }

    public Subscription OnNewLogs(string jobId, SubscriptionType subscriptionType, Func<SubscribeCallbackContext,Task> callback)
    {
        return AddSubscription(jobId, subscriptionType, callback, _loggerSubscriptions);
    }

    private void RaiseSubscriptionChanged(List<Subscription> subscriptions)
    {
        lock (_lock)
        {
            foreach (var subscription in subscriptions)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await subscription.ExecuteAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in subscription callback");
                    }
                });
            }
        }
    }


    private void RaiseSubscriptionChanged<T>(List<Subscription> subscriptions,T data)
    {
        lock (_lock)
        {
            foreach (var subscription in subscriptions)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await subscription.ExecuteAsync(data).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in subscription callback");
                    }
                });
            }
        }
    }


    private Subscription AddSubscription(string jobId, SubscriptionType subscriptionType, Func<SubscribeCallbackContext,Task> callback, List<Subscription> subscriptions)
    {
        Subscription subscription = null;
        subscription = new Subscription(jobId, subscriptionType, callback, () =>
        {
            lock (_lock)
            {
                subscriptions.Remove(subscription);
            }
        });

        lock (_lock)
        {
            subscriptions.Add(subscription);
        }

        return subscription;
    }
}