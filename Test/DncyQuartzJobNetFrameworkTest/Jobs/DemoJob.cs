using System;
using System.Threading.Tasks;
using Dncy.QuartzJob.Model;
using Quartz;

namespace DncyQuartzJobNetFrameworkTest.Jobs
{
    public class DemoJob:IBackgroundJob
    {
        /// <inheritdoc />
        public async Task Execute(IJobExecutionContext context)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            await Task.Delay(1000);
            Console.WriteLine("DemoJob is running");
            Console.ResetColor();
        }
    }
}