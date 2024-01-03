using Dotnetydd.QuartzJob.Model;
using Dotnetydd.QuartzJob.Stores;
using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnetydd.QuartzJob
{
    public class JobLogListener : IJobListener
    {
        private readonly IJobLogStore _jobLogStore;

        public JobLogListener(IJobLogStore jobLogStore)
        {
            _jobLogStore = jobLogStore;
        }


        /// <inheritdoc />
        public virtual Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            JobKey job = context.JobDetail.Key;
            bool hasException = jobException != null;
            _jobLogStore.RecordAsync(job, new JobLogModel
            {
                Time = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                RunSeconds = context.JobRunTime.Seconds,
                State = hasException ? EnumJobStates.Exception : EnumJobStates.Normal,
                Message = jobException?.Message ?? context.Result?.ToString() ?? ""
            });
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual string Name => "JobLogListener";
    }
}

