﻿using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace Dotnetydd.QuartzHost;

public class SingletonJobFactory: IJobFactory
{
    private readonly IServiceProvider _serviceProvider;

    public SingletonJobFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        return _serviceProvider.GetRequiredService<QuartzJobRunner>();
    }

    public void ReturnJob(IJob job)
    {
        (job as IDisposable)?.Dispose();
    }
}