using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dncy.QuartzJob.Model;
using Dncy.QuartzJob.Stores;
using Quartz;
using Quartz.Impl.Matchers;

namespace Dncy.QuartzJob.Utils
{
    public class SchedulerBuilderHelper
    {
        private Dictionary<string, Type> _jobDefined=new Dictionary<string, Type>();

        private IScheduler _scheduler;

        private IJobInfoStore _jobInfoStore;

        private Type _httpJobHandlerType;


        private SchedulerBuilderHelper()
        {
            _jobDefined=new Dictionary<string, Type>();
            _scheduler=null;
            _jobInfoStore=null;
            _httpJobHandlerType=null;
        }


        public static SchedulerBuilderHelper Default = new SchedulerBuilderHelper();




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


        public SchedulerBuilderHelper WithJobStore(IJobInfoStore jobStore)
        {
            _jobInfoStore = jobStore;
            return this;
        }

        /// <summary>
        /// 添加调用http服务的job处理器
        /// </summary>
        /// <param name="httpJobHandlerType">实现<see cref="IJob"/>的处理器</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public SchedulerBuilderHelper WithHTTPServiceCallJob(Type httpJobHandlerType)
        {
            if (httpJobHandlerType.GetInterface(nameof(IJob))==null)
            {
                throw new InvalidOperationException("httpJobHandlerType 的类型无效");
            }

            _httpJobHandlerType = httpJobHandlerType;
            return this;
        }


        public SchedulerBuilderHelper WithGlobalJobListener(params IJobListener[] listeners)
        {
            if (_scheduler==null)
            {
                throw new InvalidOperationException("请先设置Schedler。(WithSchedler)");
            }

            foreach (var listener in listeners)
            {
                if (listener!=null)
                {
                    _scheduler.ListenerManager.AddJobListener(listener,GroupMatcher<JobKey>.AnyGroup());
                }
            }

            return this;
        }


        public SchedulerBuilderHelper WithGlobalTriggerListener(params ITriggerListener[] listeners)
        {
            if (_scheduler==null)
            {
                throw new InvalidOperationException("请先设置Schedler。(WithSchedler)");
            }

            foreach (var listener in listeners)
            {
                if (listener!=null)
                {
                    _scheduler.ListenerManager.AddTriggerListener(listener,GroupMatcher<TriggerKey>.AnyGroup());
                }
            }

            return this;
        }


        public async Task BuildAsync()
        {
            var jobs = await _jobInfoStore.GetListAsync();
            if (jobs==null||!jobs.Any())
            {
                return;
            }
            if (_scheduler==null)
            {
                throw new InvalidOperationException("no scheduler found");
            }
            foreach (var jobInfo in jobs)
            {
                if (jobInfo.TaskType == EnumTaskType.StaticExecute)
                {
                    var type = _jobDefined.FirstOrDefault(x => x.Key == jobInfo.TaskName).Value;
                    if (type == null)
                        continue;
                    var job = JobBuilder.Create(type)
                        .WithIdentity(jobInfo.TaskName, jobInfo.GroupName)
                        .WithDescription(jobInfo.Describe)
                        .Build();
                    var triggerBuilder = TriggerBuilder.Create()
                        .WithIdentity(jobInfo.TaskName, jobInfo.GroupName)
                        .WithCronSchedule(jobInfo.Interval)
                        .StartNow();

                    var trigger = triggerBuilder.Build();
                    await _scheduler.ScheduleJob(job, trigger);
                    if (jobInfo.Status != EnumJobStates.Normal)
                    {
                        await _scheduler.PauseTrigger(trigger.Key);
                    }
                    jobInfo.TriggerName = trigger.Key.Name;
                }
                else
                {
                    var job = JobBuilder.Create(_httpJobHandlerType)
                        .WithIdentity(jobInfo.TaskName, jobInfo.GroupName)
                        .Build();
                    var triggerBuilder = TriggerBuilder.Create()
                        .WithIdentity(jobInfo.TaskName, jobInfo.GroupName)
                        .WithDescription(jobInfo.Describe)
                        .WithCronSchedule(jobInfo.Interval)
                        .StartNow();

                    var trigger = triggerBuilder.Build();
                    await _scheduler.ScheduleJob(job, trigger);
                    if (jobInfo.Status != EnumJobStates.Normal)
                    {
                        await _scheduler.PauseTrigger(trigger.Key);
                    }

                    jobInfo.TriggerName = trigger.Key.Name;
                }
            }
        }


        public void Build()
        {
            var jobs = _jobInfoStore.GetListAsync().GetAwaiter().GetResult();
            if (jobs==null||!jobs.Any())
            {
                return;
            }
            if (_scheduler==null)
            {
                throw new InvalidOperationException("no scheduler found");
            }
            foreach (var jobInfo in jobs)
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
                        .WithCronSchedule(jobInfo.Interval)
                        .StartNow();

                    var trigger = triggerBuilder.Build();
                    _scheduler.ScheduleJob(job, trigger).Wait();
                    if (jobInfo.Status != EnumJobStates.Normal)
                    {
                        _scheduler.PauseTrigger(trigger.Key).Wait();
                    }
                }
                else
                {
                    var job = JobBuilder.Create(_httpJobHandlerType)
                        .WithIdentity(jobInfo.TaskName, jobInfo.GroupName)
                        .Build();
                    var triggerBuilder = TriggerBuilder.Create()
                        .WithIdentity(jobInfo.TaskName, jobInfo.GroupName)
                        .WithDescription(jobInfo.Describe)
                        .WithCronSchedule(jobInfo.Interval)
                        .StartNow();

                    var trigger = triggerBuilder.Build();
                    _scheduler.ScheduleJob(job, trigger).Wait();

                    if (jobInfo.Status != EnumJobStates.Normal)
                    {
                        _scheduler.PauseTrigger(trigger.Key).Wait();
                    }
                }
            }
        }

    }
}

