using Dotnetydd.QuartzHost.Models;
using Dotnetydd.QuartzHost.Storage;
using Dotnetydd.QuartzHost.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnetydd.QuartzHost;

public class QuartzHostedService: IHostedService
    {

        private readonly JobDefined _jobDefined;
        private readonly IJobFactory _jobFactory;
        private readonly IJobInfoStore _jobStore;

        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public QuartzHostedService(ISchedulerFactory schedulerFactory, IJobInfoStore jobStore,
            IJobFactory jobFactory, JobDefined jobDefined, IServiceProvider serviceProvider,IConfiguration configuration)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _jobDefined = jobDefined;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _jobStore = jobStore;
        }

        private IScheduler Scheduler { get; set; }


        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;
           

            var jobs = new List<JobInfoModel>();
            var staticJob= StaticJobs().ToList()??[];
            if (staticJob.Any())
            {
                jobs.AddRange(staticJob);
            }
            var rpcjob = await _jobStore.GetListAsync();
            if (rpcjob.Any())
            {
                jobs.AddRange(rpcjob);
            }
            if (!jobs.Any())
            {
                return;
            }

            using var sc = _serviceProvider.CreateScope();
            var dataRepository = sc.ServiceProvider.GetService<DataRepository>();
            var jobListener = sc.ServiceProvider.GetService<IJobListener>();
            var triggerListener = sc.ServiceProvider.GetService<ITriggerListener>();
            foreach (var job in jobs)
            {
                await dataRepository.AddJobAsync(job);
            }

            await SchedulerBuilderHelper.Default
                .WithStaticJobTypeDefined(_jobDefined.JobDictionary)
                .WithSchedler(Scheduler)
                .WithJobStore(_jobStore)
                .WithGlobalJobListener(jobListener)
                .WithGlobalTriggerListener(triggerListener)
                .WithHTTPServiceCallJob(typeof(HttpServiceCallJob))
                .BuildAsync();

            await Scheduler.Start(cancellationToken);
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (Scheduler != null)
            {
                await Scheduler.Shutdown(cancellationToken);
            }
        }


        IEnumerable<JobInfoModel> StaticJobs()
        {
            var settings = _configuration.GetSection("Jobs").Get<List<JobSetting>>();
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
                    Status = job.Enabled ? EnumJobStates.Normal : EnumJobStates.Stopped
                };
            }
        }
    }