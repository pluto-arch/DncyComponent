﻿using Dncy.QuartzJob.Model;
using Quartz;

namespace Dncy.QuartzJobAspNetCoreTest.Jobs;

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