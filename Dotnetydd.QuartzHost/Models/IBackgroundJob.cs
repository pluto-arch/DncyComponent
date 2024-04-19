using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;

namespace Dotnetydd.QuartzHost;


interface IQuartzJob: IJob
{
}

public abstract class QuartzJob<JobImpl> : IQuartzJob
{
    private readonly ILogger<JobImpl> _logger;

    protected QuartzJob(ILogger<JobImpl> logger=null)
    {
        _logger = logger??NullLogger<JobImpl>.Instance;
    }

    /// <summary>
    /// job logic implementation
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public abstract Task ExecuteAsync(IJobExecutionContext context);

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await ExecuteAsync(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e,"{JobClass} has exception.",typeof(JobImpl).FullName);
        }
    }
}