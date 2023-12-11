#if NETCOREAPP

using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;

namespace Dotnetydd.QuartzJob
{
    public class SingletonJobFactory : IJobFactory
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
}
#endif


#if NET46
using System;
using Dncy.QuartzJob.Stores;
using Quartz;
using Quartz.Spi;

namespace Dncy.QuartzJob
{
    public class SingletonJobFactory : IJobFactory
    {
        private readonly Lazy<QuartzJobRunner> quartzJobRunner ;

        public SingletonJobFactory(IJobInfoStore jobInfoStore,IJobLogStore logStore=null)
        {
            quartzJobRunner = new Lazy<QuartzJobRunner>(() => new QuartzJobRunner(jobInfoStore, logStore));
        }


        public SingletonJobFactory()
        {
            quartzJobRunner = new Lazy<QuartzJobRunner>(() => new QuartzJobRunner());
        }

        
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return quartzJobRunner.Value;
        }

        public void ReturnJob(IJob job)
        {
            ( job as IDisposable )?.Dispose();
        }
    }
}
#endif
