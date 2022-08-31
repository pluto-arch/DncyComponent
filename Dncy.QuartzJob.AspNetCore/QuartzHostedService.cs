using Dncy.QuartzJob.Model;
using Dncy.QuartzJob.Stores;
using Dncy.QuartzJob.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;

namespace Dncy.QuartzJob.AspNetCore
{
    public class QuartzHostedService:IHostedService
    {

        private readonly JobDefined _jobDefined;
        private readonly IJobFactory _jobFactory;
        private readonly IJobInfoStore _jobStore;

        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IServiceProvider _serviceProvider;

        public QuartzHostedService(ISchedulerFactory schedulerFactory, IJobInfoStore jobStore,
            IJobFactory jobFactory, JobDefined jobDefined, IServiceProvider serviceProvider)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _jobDefined = jobDefined;
            _serviceProvider = serviceProvider;
            _jobStore = jobStore;
        }

        private IScheduler Scheduler { get; set; }






        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;
            var jobListener = _serviceProvider.GetService<IJobListener>();
            var triggerListener = _serviceProvider.GetService<ITriggerListener>();
            if (jobListener!=null)
            {
                Scheduler.ListenerManager.AddJobListener(jobListener, GroupMatcher<JobKey>.AnyGroup());
            }

            if (triggerListener != null)
            {
                Scheduler.ListenerManager.AddTriggerListener(triggerListener, GroupMatcher<TriggerKey>.AnyGroup());
            }

            var jobs = await _jobStore.GetListAsync();
            if (jobs == null || !jobs.Any())
            {
                return;
            }

            await SchedulerBuilderHelper.Default
                .WithStaticJobTypeDefined(_jobDefined.JobDictionary)
                .WithSchedler(Scheduler)
                .SchedulerJobs(jobs)
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
    }
}

