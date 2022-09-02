// See https://aka.ms/new-console-template for more information

using Dncy.QuartzJob;
using Dncy.QuartzJob.Model;
using Dncy.QuartzJob.Stores;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System.Threading;
using DncyQuartzConsoleTest;
using System.Configuration;
using System.Reflection;
using Dncy.QuartzJob.Utils;
using Microsoft.Extensions.Configuration;


var service = new ServiceCollection();

IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

service.AddSingleton(Configuration);

service.AddDncyQuartzJobCore();
service.AddStaticJobDefined();

service.AddSingleton<IJobInfoStore, JsonFileJobInfoStore>();
service.AddSingleton<IJobLogStore, InMemoryJobLogStore>();



var sp = service.BuildServiceProvider();




var fc=sp.GetRequiredService<ISchedulerFactory>();
var Scheduler = await fc.GetScheduler();

var jfc = sp.GetRequiredService<IJobFactory>();
Scheduler.JobFactory = jfc;


var jst=sp.GetRequiredService<IJobInfoStore>();


var jobss =Configuration.GetSection("JobSettings").Get<List<JobSetting>>();
if (jobss != null)
{
    foreach (JobSetting job in jobss)
    {
        if (!job.IsOpen)
        {
            continue;
        }
        jst?.AddAsync(new JobInfoModel
        {
            Id = Guid.NewGuid().ToString("N"),
            TaskType = EnumTaskType.StaticExecute,
            TaskName = job.Name,
            DisplayName = job.DisplayName,
            GroupName = job.GroupName,
            Interval = job.Cron,
            Describe = job.Description,
            Status = EnumJobStates.Normal
        });
    }
}




var jobs = await jst.GetListAsync();
if (jobs == null || !jobs.Any())
{
    return;
}

await SchedulerBuilderHelper.Default
    .WithStaticJobTypeDefined(sp.GetRequiredService<JobDefined>().JobDictionary)
    .WithSchedler(Scheduler)
    .WithJobStore(jst)
    .BuildAsync();

await Scheduler.Start();

Console.ReadKey();

