
#if !NET461

using System;
using Microsoft.Extensions.DependencyInjection;
using Pluto.Redis;
using StackExchange.Redis;

namespace Dotnetydd.StackExchangeRedis.Extensions
{
    public static class ServiceCollectionExtension
    {

        /// <summary>
        /// redis客户端工厂
        /// </summary>
        /// <param name="services"></param>
        /// <remarks>多个reidsclient的时候按照name获取实例</remarks>
        /// <returns></returns>
        public static IServiceCollection AddRedisClientFactory(this IServiceCollection services)
        {
            services.AddSingleton<RedisClientFactory>();
            return services;
        }

        /// <summary>
        /// 注入redis
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedisClient(this IServiceCollection services, Action<ConfigurationOptions> options)
        {
            services.AddSingleton<IRedisClient, RedisClient>(_ =>
            {
                var o = new ConfigurationOptions();
                options.Invoke(o);
                return new RedisClient(o);
            });
            return services;
        }

        /// <summary>
        /// 注入redis
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedisClient(this IServiceCollection services,string configuration, Action<ConfigurationOptions> options=null)
        {
            services.AddSingleton<IRedisClient, RedisClient>(_ => new RedisClient(configuration,options));
            return services;
        }

    }

}
#endif
