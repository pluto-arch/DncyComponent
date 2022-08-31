using Dncy.QuartzJob.Model;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Dncy.QuartzJob.Utils
{
    public class SchedulerBuilderHelper
    {
        private Dictionary<string, Type> _jobDefined=new Dictionary<string, Type>();

        private IScheduler _scheduler;

        private List<JobInfoModel> _jobs;

        private SchedulerBuilderHelper()
        {
            _jobDefined=new Dictionary<string, Type>();
            _scheduler=null;
            _jobs=null;
        }


        public static SchedulerBuilderHelper Default = new ();




        public SchedulerBuilderHelper WithStaticJobTypeDefined(Dictionary<string, Type> defined)
        {
            _jobDefined = defined??new Dictionary<string, Type>();
            return this;
        }


        public SchedulerBuilderHelper WithSchedler(IScheduler scheduler)
        {
            _scheduler = scheduler??throw new ArgumentNullException(nameof(scheduler));
            return this;
        }


        public SchedulerBuilderHelper SchedulerJobs(List<JobInfoModel> jobs)
        {
            _jobs = jobs;
            return this;
        }


        public async Task BuildAsync()
        {
            if (_jobs==null||!_jobs.Any())
            {
                return;
            }
            foreach (var jobInfo in _jobs)
            {
                if (jobInfo.TaskType == EnumTaskType.StaticExecute)
                {
                    var type = _jobDefined.FirstOrDefault(x => x.Key == jobInfo.TaskName).Value;
                    if (type == null)
                        continue;
                    IJobDetail job = JobBuilder.Create(type)
                        .WithIdentity(jobInfo.TaskName, jobInfo.GroupName)
                        .WithDescription(jobInfo.Describe)
                        .Build();
                    var triggerBuilder = TriggerBuilder.Create()
                        .WithIdentity(jobInfo.TaskName, jobInfo.GroupName)
                        .WithCronSchedule(jobInfo.Interval);
                    if (jobInfo.Status == EnumJobStates.Normal)
                    {
                        triggerBuilder.StartNow();
                    }
                    await _scheduler.ScheduleJob(job, triggerBuilder.Build());

                }
                else
                {
                    //IJobDetail job = JobBuilder.Create<HttpResultfulJob>()
                    //    .WithIdentity(jobInfo.TaskName, jobInfo.GroupName)
                    //    .Build();
                    //var triggerBuilder = TriggerBuilder.Create()
                    //    .WithIdentity(jobInfo.TaskName, jobInfo.GroupName)
                    //    .WithDescription(jobInfo.Describe)
                    //    .WithCronSchedule(jobInfo.Interval);
                    //if (jobInfo.Status == EnumJobStates.Normal)
                    //{
                    //    triggerBuilder.StartNow();
                    //}
                    //await _scheduler.ScheduleJob(job, triggerBuilder.Build());
                }
            }
        }


        public void Build()
        {
            if (_jobs==null||!_jobs.Any())
            {
                return;
            }
            if (_scheduler==null)
            {
                return;
            }
            foreach (var jobInfo in _jobs)
            {
                if (jobInfo.TaskType == EnumTaskType.StaticExecute)
                {
                    var type = _jobDefined.FirstOrDefault(x => x.Key == jobInfo.TaskName).Value;
                    if (type == null)
                        continue;
                    IJobDetail job = JobBuilder.Create(type)
                        .WithIdentity(jobInfo.TaskName, jobInfo.GroupName)
                        .WithDescription(jobInfo.Describe)
                        .Build();
                    var triggerBuilder = TriggerBuilder.Create()
                        .WithIdentity(jobInfo.TaskName, jobInfo.GroupName)
                        .WithCronSchedule(jobInfo.Interval);
                    if (jobInfo.Status == EnumJobStates.Normal)
                    {
                        triggerBuilder.StartNow();
                    }
                    _scheduler.ScheduleJob(job, triggerBuilder.Build()).Wait();
                }
                else
                {
                    //IJobDetail job = JobBuilder.Create<HttpResultfulJob>()
                    //    .WithIdentity(jobInfo.TaskName, jobInfo.GroupName)
                    //    .Build();
                    //var triggerBuilder = TriggerBuilder.Create()
                    //    .WithIdentity(jobInfo.TaskName, jobInfo.GroupName)
                    //    .WithDescription(jobInfo.Describe)
                    //    .WithCronSchedule(jobInfo.Interval);
                    //if (jobInfo.Status == EnumJobStates.Normal)
                    //{
                    //    triggerBuilder.StartNow();
                    //}
                    //await _scheduler.ScheduleJob(job, triggerBuilder.Build());
                }
            }
        }

    }
}

