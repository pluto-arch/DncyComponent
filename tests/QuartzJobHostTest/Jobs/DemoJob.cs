using Dotnetydd.QuartzHost;
using Quartz;

namespace QuartzJobHostTest.Jobs;

public class DemoJob:IQuartzJob
{
    private readonly ILogger<DemoJob> _logger;

    public DemoJob(ILogger<DemoJob> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        await Task.Delay(1000);
        _logger.LogDebug($"DemoJob执行完毕,所有更改已保存 - {Thread.CurrentThread.ManagedThreadId}");
    }
}