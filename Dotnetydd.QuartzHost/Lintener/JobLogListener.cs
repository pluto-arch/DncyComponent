using Dotnetydd.QuartzHost.Models;
using Dotnetydd.QuartzHost.Storage;
using Quartz;

namespace Dotnetydd.QuartzHost.Lintener;

public class JobLogListener: IJobListener
{
    private readonly DataRepository _dataRepository;

    public JobLogListener(DataRepository dataRepository)
    {
        _dataRepository = dataRepository;
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
    public virtual async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
    {
        JobKey job = context.JobDetail.Key;
        bool hasException = jobException != null;
        var log = new JobLogModel
        {
            JobKey = $"{job.Group}:{job.Name}",
            Time = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            RunSeconds = context.JobRunTime.Seconds,
            State = hasException ? EnumJobStates.Exception : EnumJobStates.Normal,
            Message = jobException?.Message ?? context.Result?.ToString() ?? ""
        };
        await _dataRepository.AddJobLogAsync(log);
    }

    /// <inheritdoc />
    public virtual string Name => "JobLogListener";
}