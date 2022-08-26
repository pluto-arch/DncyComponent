using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dncy.RateLimit.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRequestPathLimitTargetResolver(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddTransient<IRateLimitTargetResolver, RequestPathRateLimitResolver>();
            return services;
        }
    }
}

