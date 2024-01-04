using Dotnetydd.QuartzJob;
using Quartz;

namespace QuartzJobTest.Jobs
{
    public class UserJob:IBackgroundJob
{
    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        await Task.Delay(1000);
        Console.WriteLine("UserJob is running");
        context.Result = "UserJob执行完毕";
    }
}
}
