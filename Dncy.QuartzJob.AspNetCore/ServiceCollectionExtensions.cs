using Dotnetydd.QuartzJob.AspNetCore.Handlers;

namespace Dotnetydd.QuartzJob.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDncyQuartzJobWithUI(this IServiceCollection services)
        {
            services.AddScoped<QuartzJobUiMiddleware>();
            services.AddHostedService<QuartzHostedService>();
            services.AddTransient<JobDataHandler>();
            return services;
        }



        public static IApplicationBuilder UseDncyQuartzJobUI(this IApplicationBuilder app)
        {
            app.UseMiddleware<QuartzJobUiMiddleware>();
            return app;
        }
    }
}

