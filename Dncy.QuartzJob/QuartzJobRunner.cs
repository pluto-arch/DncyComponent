using Dotnetydd.QuartzJob.Constants;
using Dotnetydd.QuartzJob.Model;
/* 项目“Dotnetydd.QuartzJob (net7.0)”的未合并的更改
在此之前:
using Dotnetydd.QuartzJob.Stores;
在此之后:
using Dotnetydd.QuartzJob.Stores;
using Dncy;
using Dncy.QuartzJob;
using Dotnetydd.QuartzJob;
*/
using Dotnetydd.QuartzJob.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;
using System;
using System.Threading.Tasks;




namespace Dotnetydd.QuartzJob
{
    public class QuartzJobRunner : IJob
    {

        private readonly ILogger<QuartzJobRunner> _logger;

        private readonly IServiceProvider _serviceProvider;

        public QuartzJobRunner(IServiceProvider serviceProvider, ILogger<QuartzJobRunner> logger = null)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? NullLogger<QuartzJobRunner>.Instance;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobType = context.JobDetail.JobType;
            JobInfoModel jobModel = null;
            using (var scope = _serviceProvider.CreateScope())
            {
                var job = context.JobDetail.Key;
                var jobInfoStore = scope.ServiceProvider.GetService<IJobInfoStore>() ?? new InMemoryJobInfoStore();
                var jobLogStore = scope.ServiceProvider.GetService<IJobLogStore>() ?? NullJobLogStore.Instrance;
                try
                {
                    jobModel = await jobInfoStore.GetAsync(job);
                    if (scope.ServiceProvider.GetRequiredService(jobType) is not IJob jobToExecute)
                    {
                        _logger.LogWarning("no {jobType} found !", jobType.Name);
                    }
                    else
                    {
                        if (jobModel == null)
                        {
                            _logger.LogWarning("no {jobType} found in store!", jobType.Name);
                            return;
                        }

                        if (jobModel.Status != EnumJobStates.Normal)
                        {
                            _logger.LogWarning("{jobType} is not normal to run!", jobType.Name);
                            return;
                        }

                        context.Put(JobExecutionContextConstants.JobExecutionContextData_JobInfo, jobModel);
                        await jobToExecute.Execute(context);
                        _logger.LogInformation("{jobType} has been executed, ", jobType.Name);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "{jobType} running has an error : {@message}", jobType.Name, e.Message);
                    await jobLogStore.RecordAsync(job, new JobLogModel
                    {
                        Time = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                        RunSeconds = context.JobRunTime.Seconds,
                        State = EnumJobStates.Exception,
                        Message = e.Message
                    });
                    if (jobModel != null)
                    {
                        jobModel.Status = EnumJobStates.Exception;
                        await jobInfoStore.UpdateAsync(jobModel);
                        await context.Scheduler.PauseJob(job);
                    }
                }
            }
        }

    }
}

