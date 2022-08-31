using Dncy.QuartzJob.Model;
using Dncy.QuartzJob.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;

namespace Dncy.QuartzJob
{
    public class QuartzJobRunner: IJob
    {
        private readonly ILogger<QuartzJobRunner> _logger;

        private readonly IServiceProvider _serviceProvider;

        public QuartzJobRunner(IServiceProvider serviceProvider, ILogger<QuartzJobRunner> logger=null)
        {
            _serviceProvider = serviceProvider??throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger??NullLogger<QuartzJobRunner>.Instance;
        }



        public async Task Execute(IJobExecutionContext context)
        {
            var jobType = context.JobDetail.JobType;
            using (var scope = _serviceProvider.CreateScope())
            {
                var job = context.JobDetail.Key;
                var jobInfoStore = scope.ServiceProvider.GetService<IJobInfoStore>() ?? new InMemoryJobStore();
                var jobLogStore = scope.ServiceProvider.GetService<IJobLogStore>() ?? NullJobLog.Instrance;
                try
                {
                    if (scope.ServiceProvider.GetRequiredService(jobType) is not IJob jobToExecute)
                    {
                        _logger.LogWarning("no {jobType} found !", jobType.Name);
                    }
                    else
                    {
                        await jobToExecute.Execute(context);
                        _logger.LogInformation("{jobType} has been executed, ", jobType.Name);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "job running has an error : {@message}", e.Message);
                    await jobLogStore.RecordAsync(job, new JobLogModel
                    {
                        Time = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                        RunSeconds = context.JobRunTime.Seconds,
                        State = EnumJobStates.Exception,
                        Message = e.Message
                    });
                    var jobModel = await jobInfoStore.GetAsync(job);
                    jobModel.Status = EnumJobStates.Exception;
                    await jobInfoStore.UpdateAsync(jobModel);
                    await context.Scheduler.PauseJob(job);
                }
            }
        }

    }
}

