using Dncy.QuartzJob.Model;
using Dncy.QuartzJob.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl.Triggers;
using Quartz.Spi;

namespace Dncy.QuartzJob.AspNetCore.Handler
{
    internal class JobDataHandler
    {
        private readonly IJobInfoStore _jobInfoStore;
        private readonly IJobFactory _jobFactory;
        private readonly ISchedulerFactory _jobSchedularFactory;
        private readonly IJobLogStore _jobLogStore;
        private readonly ILogger<JobDataHandler> _logger;

        public JobDataHandler(IJobInfoStore jobInfoStore, IJobFactory jobFactory, ISchedulerFactory jobSchedularFactory, IJobLogStore jobLogStore, ILogger<JobDataHandler> logger)
        {
            _jobInfoStore = jobInfoStore;
            _jobFactory = jobFactory;
            _jobSchedularFactory = jobSchedularFactory;
            _jobLogStore = jobLogStore;
            _logger = logger;
        }


        internal async Task<JobDataResult<DashboardDataResult>> DashboardData()
        {
            var res = new DashboardDataResult();
            var jobs = await _jobInfoStore.GetListAsync();
            res.TotalCount = (uint)jobs.Count;
            res.RunningCount = (uint)(jobs?.Count(x => x.Status == EnumJobStates.Normal) ?? 0);
            res.PauseCount = (uint)(jobs?.Count(x => x.Status == EnumJobStates.Pause) ?? 0);
            return new JobDataResult<DashboardDataResult> {Data = res};
        }



        internal async Task<JobDataResult<List<JobInfoModel>>> Tasks()
        {
            var jobs = await _jobInfoStore.GetListAsync();
            jobs = jobs?.OrderBy(x => x.Id)?.ToList();
            IScheduler scheduler = await _jobSchedularFactory.GetScheduler();
            foreach (JobInfoModel job in jobs ?? new List<JobInfoModel>())
            {
                IReadOnlyCollection<ITrigger> triggers =
                    await scheduler.GetTriggersOfJob(JobKey.Create(job.TaskName, job.GroupName));
                foreach (ITrigger trigger in triggers)
                {
                    DateTimeOffset? dateTimeOffset = trigger.GetPreviousFireTimeUtc();
                    if (!dateTimeOffset.HasValue)
                    {
                        continue;
                    }
                    job.TriggerName = trigger.Key.Name;
                    job.LastRunTime = $"{dateTimeOffset.Value.LocalDateTime:yyyy-MM-dd HH:mm:ss}";
                    job.TriggerStatus = await scheduler.GetTriggerState(trigger.Key);
                }
            }

            return new JobDataResult<List<JobInfoModel>> {Data = jobs,Count=jobs?.Count ?? 0};
        }

        public async Task<JobDataResult<string>> PauseTask(string id)
        {
            JobInfoModel job = await _jobInfoStore.GetAsync(id);
            if (job == null)
            {
                return new JobDataResult<string>{Code=-1,Msg="job不存在"};
            }

            IScheduler scheduler = await _jobSchedularFactory.GetScheduler();
            JobKey jk = JobKey.Create(job.TaskName, job.GroupName);
            await scheduler.PauseJob(jk);
            job.Status = EnumJobStates.Pause;
            await _jobInfoStore.UpdateAsync(job);
            await _jobLogStore.RecordAsync(jk,
                new JobLogModel
                {
                    Time = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    RunSeconds = 0,
                    State = EnumJobStates.Pause,
                    Message = "暂停指令发送成功"
                });

            return new JobDataResult<string>{Code=0,Msg="操作成功"};
        }

        public async Task<JobDataResult<List<JobLogModel>>> JobLogs(string id,int pageNo)
        {
            JobInfoModel job = await _jobInfoStore.GetAsync(id);
            if (job == null)
            {
                return new JobDataResult<List<JobLogModel>>{Code=-1,Msg="日志为空"};
            }

            JobKey jk = JobKey.Create(job.TaskName, job.GroupName);
            List<JobLogModel> logs = await _jobLogStore.GetListAsync(jk,pageNo,20);
            return new JobDataResult<List<JobLogModel>>{Data=logs};
        }

        public async Task<JobDataResult<string>> AddJob(string body)
        {
            var model = JsonConvert.DeserializeObject<CreateJobModel>(body);
            if (model==null)
            {
                return new JobDataResult<string>{Code=-1,Msg="请求参数错误"};
            }
            var job = new JobInfoModel
            {
                Id = Guid.NewGuid().ToString("N"),
                TaskType = EnumTaskType.DynamicExecute,
                TaskName = model.Name,
                DisplayName = model.DisplayName,
                GroupName = model.GroupName,
                Interval = model.Interval,
                Describe = model.Desc,
                Status = EnumJobStates.Stopped,
                ApiUrl = model.CallUrl
            };
            await _jobInfoStore.AddAsync(job);
            var res = await AddNewJob(model);
            if (!res)
            {
                return new JobDataResult<string>{Code=-1,Msg="创建失败"};
            }

            job.Status = EnumJobStates.Normal;
            await _jobInfoStore.UpdateAsync(job);

            return new JobDataResult<string>{Code=0,Msg="操作成功"};
        }

        public async Task<JobDataResult<string>> Refire(string id)
        {
            JobInfoModel job = await _jobInfoStore.GetAsync(id);
            if (job == null)
            {
                return new JobDataResult<string>{Code=-1,Msg="job不存在"};
            }

            IScheduler scheduler = await _jobSchedularFactory.GetScheduler();
            JobKey jk = JobKey.Create(job.TaskName, job.GroupName);
            await _jobLogStore.RecordAsync(jk,
                new JobLogModel
                {
                    Time = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    RunSeconds = 0,
                    State = EnumJobStates.Normal,
                    Message = "重启指令发送成功"
                });
            await scheduler.ResumeJob(jk);
            job.Status = EnumJobStates.Normal;
            await _jobInfoStore.UpdateAsync(job);

            return new JobDataResult<string>{Code=0,Msg="执行成功"};
        }

        public async Task<JobDataResult<string>> Execute(string id)
        {
            JobInfoModel job = await _jobInfoStore.GetAsync(id);
            if (job == null)
            {
                return new JobDataResult<string>{Code=-1,Msg="job不存在"};
            }
            JobKey jobKey = JobKey.Create(job.TaskName, job.GroupName);
            IScheduler scheduler = await _jobSchedularFactory.GetScheduler();
            await scheduler.TriggerJob(jobKey);
            await _jobLogStore.RecordAsync(jobKey,
                new JobLogModel
                {
                    Time = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    RunSeconds = 0,
                    State = EnumJobStates.Normal,
                    Message = "发送立即执行指令成功"
                });
            return new JobDataResult<string>{Code=0,Msg="执行成功"};
        }


        private async Task<bool> AddNewJob(CreateJobModel model)
        {
            try
            {
                var (success, _) = IsValidExpression(model.Interval);
                if (!success)
                {
                    return false;
                }
                IJobDetail job = JobBuilder.Create<HttpServiceCallJob>()
                    .WithIdentity(model.Name, model.GroupName)
                    .Build();
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(model.Name, model.GroupName)
                    .StartNow()
                    .WithDescription(model.Desc)
                    .WithCronSchedule(model.Interval)
                    .Build();
                IScheduler scheduler = await _jobSchedularFactory.GetScheduler();

                scheduler.JobFactory = _jobFactory;

                await scheduler.ScheduleJob(job, trigger);
                await scheduler.Start();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "message: {msg}", e.Message);
                return false;
            }
        }

        static (bool, string) IsValidExpression(string cronExpression)
        {
            try
            {
                CronTriggerImpl trigger = new()
                {
                    CronExpressionString = cronExpression
                };
                DateTimeOffset? date = trigger.ComputeFirstFireTimeUtc(null);
                return (date != null, date == null ? $"请确认表达式{cronExpression}是否正确!" : "");
            }
            catch (Exception e)
            {
                return (false, $"请确认表达式{cronExpression}是否正确!{e.Message}");
            }
        }
    }
}

