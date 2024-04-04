using Dotnetydd.QuartzHost.Models;
using Dotnetydd.QuartzHost.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;
using Quartz.Spi;
using static Quartz.Logging.OperationName;

namespace Dotnetydd.QuartzHost;

public class SingletonJobFactory: IJobFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SingletonJobFactory> _logger;

    public SingletonJobFactory(IServiceProvider serviceProvider,ILogger<SingletonJobFactory> logger=null)
    {
        _serviceProvider = serviceProvider;
        _logger = logger??NullLogger<SingletonJobFactory>.Instance;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        try
        {
            var jobDetail = bundle.JobDetail;
            var jobType = jobDetail.JobType;
            var jobInfoStore = _serviceProvider.GetService<IJobInfoStore>()??new InMemoryJobInfoStore();
            var jobInfo = jobInfoStore.Get(bundle.JobDetail.Key);
            if (_serviceProvider.GetRequiredService(jobType) is not IJob jobToExecute)
            {
                _logger.LogError("Problem instantiating class '{JobClassName}'",bundle.JobDetail.JobType.FullName);
                throw new SchedulerException($"Problem instantiating class '{bundle.JobDetail.JobType.FullName}'");
            }
            bundle.JobDetail.JobDataMap[JobExecutionContextConstants.JOBINFO_KEY] = jobInfo;
            return jobToExecute;
        }
        catch (Exception ex)
        {
            _logger.LogError("Problem instantiating class '{JobClassName}'",bundle.JobDetail.JobType.FullName);
            throw new SchedulerException($"Problem instantiating class '{bundle.JobDetail.JobType.FullName}'", ex);
        }


        //return _serviceProvider.GetRequiredService<QuartzJobRunner>();
    }

    public void ReturnJob(IJob job)
    {
        (job as IDisposable)?.Dispose();
    }
}