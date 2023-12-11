
#if NETCOREAPP
using Dotnetydd.QuartzJob.Model;
using Dotnetydd.QuartzJob.Stores;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Spi;
using System;
using System.Linq;
using System.Reflection;

namespace Dotnetydd.QuartzJob
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDncyQuartzJobCore(this IServiceCollection services)
        {
            services.AddSingleton<QuartzJobRunner>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddTransient<IJobStore, RAMJobStore>();
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddTransient<IJobListener, JobLogListener>();
            services.AddTransient<HttpServiceCallJob>();
            return services;
        }


        /// <summary>
        /// 添加json文件存储器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddJsonFileJobInfoStore(this IServiceCollection services)
        {
            services.AddSingleton<IJobInfoStore, JsonFileJobInfoStore>();
            return services;
        }


        /// <summary>
        /// 添加内存job信息存储器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInMemoryJobInfoStore(this IServiceCollection services)
        {
            services.AddSingleton<IJobInfoStore, InMemoryJobInfoStore>();
            return services;
        }


        /// <summary>
        /// 添加内存日志存储器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInMemoryLogStore(this IServiceCollection services)
        {
            services.AddSingleton<IJobLogStore, InMemoryJobLogStore>();
            return services;
        }



        /// <summary>
        /// 初始化静态job的类型定义
        /// </summary>
        /// <param name="services"></param>
        public static void AddStaticJobDefined(this IServiceCollection services)
        {
            var jobd = new JobDefined
            {
                JobDictionary = new()
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
#endif

