using Dncy.QuartzJob.Model;
using Quartz;

namespace DncyQuartzConsoleTest;

public class HttpResultfulJob : IJob,IBackgroundJob
{
    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        await Task.Delay(1000);
        Console.WriteLine("hello");
    }
}