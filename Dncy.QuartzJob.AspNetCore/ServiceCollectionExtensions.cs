using Dotnetydd.QuartzJob.AspNetCore.Handlers;
using Dotnetydd.QuartzJob.AspNetCore.Options;

namespace Dotnetydd.QuartzJob.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQuartzJobWithDashboard(this IServiceCollection services,Action<JobDashboardOptions> optionAction)
        {
            services.Configure<JobDashboardOptions>(optionAction);
            services.AddScoped<QuartzJobUiMiddleware>();
            services.AddHostedService<QuartzHostedService>();
            services.AddTransient<JobDataHandler>();
            return services;
        }



        public static IApplicationBuilder UseQuartzJobDashboard(this IApplicationBuilder app)
        {
            app.UseMiddleware<QuartzJobUiMiddleware>();
            return app;
        }
    }
}

