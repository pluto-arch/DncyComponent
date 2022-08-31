using System.Reflection;
using Dncy.QuartzJob.Model;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Impl;
using Quartz;
using Quartz.Simpl;
using Quartz.Spi;

namespace Dncy.QuartzJob
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDncyQuartzJobCore(this IServiceCollection services)
        {
            services.AddTransient<QuartzJobRunner>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddTransient<IJobStore, RAMJobStore>();
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            return services;
        }


        public static void AddStaticJobs(this IServiceCollection services)
        {
            var jobd = new JobDefined
            {
                JobDictionary=new()
            };
            var assembly = Assembly.GetEntryAssembly();
            var baceType = typeof(IBackgroundJob);
            var implTypes = assembly.GetTypes().Where(c => c != baceType && baceType.IsAssignableFrom(c)).ToList();
            if (!implTypes.Any())
            {
                services.AddSingleton(jobd);
                return;
            }

            foreach (Type impltype in implTypes)
            {
                jobd.JobDictionary.Add(impltype.Name, impltype);
                services.AddTransient(impltype);
            }

            services.AddSingleton(jobd);
        }
    }
}

