using Dotnetydd.QuartzHost;
using Quartz;

namespace QuartzJobHostTest.Jobs;

public class UserJob: IBackgroundJob
{
    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        await Task.Delay(1000);
        Console.WriteLine("UserJob is running");
        context.Result = "UserJob执行完毕";
    }
}