using Dncy.QuartzJob.Model;
using Quartz;

namespace Dncy.QuartzJobAspNetCoreTest.Jobs;

public class DemoJob:IBackgroundJob,IJob
{
    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        await Task.Delay(1000);
        Console.WriteLine("DemoJob is running");
    }
}