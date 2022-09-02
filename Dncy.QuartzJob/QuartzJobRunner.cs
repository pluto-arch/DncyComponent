using System;
using Dncy.QuartzJob.Model;
using Dncy.QuartzJob.Stores;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Dncy.QuartzJob.Constants;
#if NET6_0
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
#endif
using Quartz;

#if NET46
using Serilog;
#endif


namespace Dncy.QuartzJob
{
    public class QuartzJobRunner : IJob
    {

#if NET6_0
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

                        if (jobModel.Status!=EnumJobStates.Normal)
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
                    _logger.LogError(e, "{jobType} running has an error : {@message}",jobType.Name, e.Message);
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
#endif


#if NET46
        private readonly IJobInfoStore _jobInfoStore;
        private readonly IJobLogStore _jobLogStore;
        private static ConcurrentDictionary<Type,IJob> JobInstrances = new ConcurrentDictionary<Type,IJob>();


        public QuartzJobRunner(IJobInfoStore jobInfoStore,IJobLogStore jobLogStore)
        {
            _jobInfoStore = jobInfoStore??new InMemoryJobInfoStore();
            _jobLogStore = jobLogStore ?? NullJobLogStore.Instrance;
        }

        public QuartzJobRunner()
        {
            _jobInfoStore = new InMemoryJobInfoStore();
            _jobLogStore = NullJobLogStore.Instrance;
        }



        public async Task Execute(IJobExecutionContext context)
        {
            var jobType = context.JobDetail.JobType;
            var job = context.JobDetail.Key;
            JobInfoModel jobModel = null;
            try
            {
                jobModel=await _jobInfoStore.GetAsync(job);
                if (jobModel == null)
                {
                    Log.Logger.Warning("no {jobType} found in store!", jobType.Name);
                    return;
                }

                if (jobModel.Status!=EnumJobStates.Normal)
                {
                    Log.Logger.Warning("{jobType} cannot run in {status}!", jobType.Name,jobModel.Status);
                    return;
                }

                if (jobType?.GetInterface(nameof(IJob)) == null)
                {
                    Log.Logger.Warning("no {jobType} found !", jobType.Name);
                    return;
                }
                Log.Logger.Debug("{jobType} executing !", jobType.Name);
                context.Put(JobExecutionContextConstants.JobExecutionContextData_JobInfo, jobModel);
                if (JobInstrances.TryGetValue(jobType,out var jobIns))
                {
                    await jobIns.Execute(context);
                }
                else
                {
                    jobIns=(IJob)Activator.CreateInstance(jobType);
                    JobInstrances.AddOrUpdate(jobType,x=>jobIns,(k,v)=>jobIns);
                    await jobIns.Execute(context);
                }
                Log.Logger.Debug("{jobType} has been executed !", jobType.Name);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "{jobType} running has an error : {@message}",jobType.Name, e.Message);
                await _jobLogStore.RecordAsync(job, new JobLogModel
                {
                    Time = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    RunSeconds = context.JobRunTime.Seconds,
                    State = EnumJobStates.Exception,
                    Message = e.Message
                });
                if (jobModel!=null)
                {
                    jobModel.Status = EnumJobStates.Exception;
                    //await _jobInfoStore.UpdateAsync(jobModel);
                    //await context.Scheduler.PauseJob(job);
                }
            }

        }

#endif





    }
}

