using Dotnetydd.QuartzHost.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dotnetydd.QuartzHost.Storage;

public class DataRepository
{
    private readonly IConfiguration _configuration;
    private readonly IJobInfoStore _jobInfoStore;
    private readonly object _lock = new();
    private readonly ILogger _logger;

    private readonly List<JobInfoModel> _staticJobs = new();
    
    private readonly List<Subscription> _applicationSubscriptions = new();
    
    public DataRepository(
        IConfiguration configuration, 
        IJobInfoStore jobInfoStore,
        ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(typeof(DataRepository));
        _configuration = configuration;
        _jobInfoStore = jobInfoStore;
        _staticJobs.AddRange(StaticJobs());
    }


    public async Task<List<JobInfoModel>> GetJobsAsync()
    {
        var list = await _jobInfoStore.GetListAsync();
        _staticJobs.AddRange(list);
        return _staticJobs;
    }
    
    
    public void AddJob(JobInfoModel jobInfoModel)
    {
        _staticJobs.Add(jobInfoModel);
        RaiseSubscriptionChanged(_applicationSubscriptions);
    }
    
    
    public Subscription OnNewApplications(Func<Task> callback)
    {
        return AddSubscription(string.Empty, SubscriptionType.Read, callback, _applicationSubscriptions);
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
    
    private Subscription AddSubscription(string applicationId, 
        SubscriptionType subscriptionType, Func<Task> callback, List<Subscription> subscriptions)
    {
        Subscription subscription = null;
        subscription = new Subscription(applicationId, subscriptionType, callback, () =>
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
    
    IEnumerable<JobInfoModel> StaticJobs()
    {
        var settings=_configuration.GetSection("Jobs").Get<List<JobSetting>>();
        foreach (var job in settings)
        {
            yield return new JobInfoModel
            {
                Id = Guid.NewGuid().ToString("N"),
                TaskType = EnumTaskType.StaticExecute,
                TaskName = job.Name,
                DisplayName = job.DisplayName,
                GroupName = job.GroupName,
                Interval = job.Cron,
                Describe = job.Description,
                Status = job.IsOpen ? EnumJobStates.Normal : EnumJobStates.Stopped
            };
        }
    }

}