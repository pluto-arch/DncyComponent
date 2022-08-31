using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dncy.QuartzJob.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDncyQuartzJobWithUI(this IServiceCollection services)
        {
            services.AddScoped<QuartzJobUiMiddleware>();
            services.AddHostedService<QuartzHostedService>();
            return services;
        }



        public static IApplicationBuilder UseDncyQuartzJobUI(this IApplicationBuilder app)
        {
            app.UseMiddleware<QuartzJobUiMiddleware>();
            return app;
        }
    }
}

