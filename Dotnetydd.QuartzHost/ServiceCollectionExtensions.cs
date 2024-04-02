using System.Reflection;
using Dotnetydd.QuartzHost.Lintener;
using Dotnetydd.QuartzHost.Models;
using Dotnetydd.QuartzHost.Storage;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Spi;

namespace Dotnetydd.QuartzHost;

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
    /// 初始化静态job的类型定义
    /// </summary>
    public static void AddStaticJobDefined(this IServiceCollection services,params Assembly[] staticJobAssemblys)
    {
        var jobd = new JobDefined
        {
            JobDictionary = new()
        };
        foreach (var assembly in staticJobAssemblys)
        {
            var baceType = typeof(IQuartzJob);
            var implTypes = assembly.GetTypes().Where(c => c != baceType && baceType.IsAssignableFrom(c)).ToList();
            if (!implTypes.Any())
            {
                services.AddSingleton(jobd);
                return;
            }

            foreach (var impltype in implTypes)
            {
                jobd.JobDictionary.Add(impltype.Name, impltype);
                services.AddTransient(impltype);
            }
            services.AddSingleton(jobd);
        }
    }
}