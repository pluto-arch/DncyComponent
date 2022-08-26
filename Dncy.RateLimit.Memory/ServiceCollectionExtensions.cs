using Dncy.RateLimit.Core.Algorithms;
using Dncy.RateLimit.Core.Rules;
using Dncy.RateLimit.MemoryCache.Algorithms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dncy.RateLimit.MemoryCache
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFixedWindowAlgorithm(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddSingleton<ILimitAlgorithm, FixedWindowAlgorithm>();
            return services;
        }
    }
}

