using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dncy.QuartzJob;
using Dncy.QuartzJob.Model;
using Dncy.QuartzJob.Stores;
using Dncy.QuartzJob.Utils;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Serilog;

namespace DncyQuartzJobNetFrameworkTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("ApplicationName", "DncyQuartzJobNetFrameworkTest")
                .WriteTo.Console(Serilog.Events.LogEventLevel.Verbose)
                .CreateLogger(); ;



            var jobInfoStore = new JsonFileJobInfoStore();
            var LogStore = new InMemoryJobLogStore();

           

            var dd = new StdSchedulerFactory();
            var Scheduler=await dd.GetScheduler();
            var factory = new SingletonJobFactory(jobInfoStore,LogStore);
            Scheduler.JobFactory = factory;

            await jobInfoStore.AddAsync(new JobInfoModel
            {
                Id = Guid.NewGuid().ToString("N"),
                TaskType = EnumTaskType.DynamicExecute,
                TaskName = "调用百度",
                DisplayName = "调用百度",
                GroupName = "Default",
                Interval = "0/2 * * * * ?",
                Describe = "调用百度",
                Status = EnumJobStates.Normal,
                ApiUrl = "https://www.baidu.com"
            });

            await jobInfoStore.AddAsync(new JobInfoModel
            {
                Id = Guid.NewGuid().ToString("N"),
                TaskType = EnumTaskType.DynamicExecute,
                TaskName = "callapi2222",
                DisplayName = "callapi2222",
                GroupName = "Default",
                Interval = "0/3 * * * * ?",
                Describe = "callapi2222",
                Status = EnumJobStates.Normal,
                ApiUrl = "https://www.bing.com/"
            });


            await SchedulerBuilderHelper.Default
                .WithStaticJobTypeDefined(AddStaticJobs().JobDictionary)
                .WithSchedler(Scheduler)
                .WithJobStore(jobInfoStore)
                .WithHTTPServiceCallJob(typeof(HttpServiceCallJob))
                .BuildAsync();


            await Scheduler.Start();


            Console.WriteLine("job 总数："+await jobInfoStore.CountAsync());

            Console.CancelKeyPress += (o, e) =>
            {
                jobInfoStore.SaveAllAsync().Wait();
                Process.GetCurrentProcess().Kill();
            };

            Thread.Sleep(TimeSpan.FromSeconds(600));

        }


        static JobDefined AddStaticJobs()
        {
            var jobd = new JobDefined
            {
                JobDictionary=new Dictionary<string, Type>()
            };
            var assembly = Assembly.GetEntryAssembly();
            var baceType = typeof(IBackgroundJob);
            var implTypes = assembly.GetTypes().Where(c => c != baceType && baceType.IsAssignableFrom(c)).ToList();
            if (!implTypes.Any())
            {
                return jobd;
            }

            foreach (Type impltype in implTypes)
            {
                jobd.JobDictionary.Add(impltype.Name, impltype);
            }

            return jobd;
        }
    }
}
