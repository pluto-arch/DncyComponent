using System.Collections;
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

public static class Extensions
{
    public static IServiceCollection AddDncyQuartzJobCore(this IServiceCollection services)
    {
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

    public static JobInfoModel GetJobInfo(this IJobExecutionContext ctx)
    {
        if (ctx.JobDetail.JobDataMap.ContainsKey(JobExecutionContextConstants.JOBINFO_KEY))
        {
            return ctx.JobDetail.JobDataMap.Get(JobExecutionContextConstants.JOBINFO_KEY) as JobInfoModel;
        }
        return null;
    }

    /// <summary>
    /// 将对象转换成字典
    /// </summary>
    /// <param name="value"></param>
    internal static Dictionary<string, object> ToDictionary(this object value)
    {
        var dictionary = new Dictionary<string, object>();
        if (value != null)
        {
            if (value is IDictionary dic)
            {
                foreach (DictionaryEntry e in dic)
                {
                    dictionary.Add(e.Key.ToString()!, e.Value);
                }
                return dictionary;
            }

            foreach (var property in value.GetType().GetProperties())
            {
                var obj = property.GetValue(value, null);
                dictionary.Add(property.Name, obj);
            }
        }

        return dictionary;
    }
}