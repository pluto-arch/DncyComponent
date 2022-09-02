using System;
using System.Threading.Tasks;
using Dncy.QuartzJob.Model;
using Quartz;

namespace DncyQuartzJobNetFrameworkTest.Jobs
{
    public class UserJob:IBackgroundJob
    {
        /// <inheritdoc />
        public async Task Execute(IJobExecutionContext context)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            await Task.Delay(1000);
            Console.WriteLine("UserJob is running");
            Console.ResetColor();
        }
    }
}