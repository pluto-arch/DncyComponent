using Dotnetydd.QuartzJob;
using Quartz;

namespace QuartzJobTest.Jobs
{
    public class DemoJob:IBackgroundJob
    {
        /// <inheritdoc />
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Delay(1000);
            Console.WriteLine("DemoJob is running");
            context.Result = "DemoJob执行完毕,所有更改已保存";
        }
    }
}
