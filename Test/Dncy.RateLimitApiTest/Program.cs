using Dncy.RateLimit.AspNetCore;
using Dncy.RateLimit.AspNetCore.Options;
using Dncy.RateLimit.MemoryCache;

namespace Dncy.RateLimitApiTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();


            #region 别人的

            //builder.Services.AddMemoryCache();
            //builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            //// inject counter and rules stores
            //builder.Services.AddInMemoryRateLimiting();
            //builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            #endregion


            #region 我的


            builder.Services.Configure<LimitConfigurationOption>(builder.Configuration);
            builder.Services.AddFixedWindowAlgorithm(builder.Configuration);
            builder.Services.AddRequestPathLimitTargetResolver(builder.Configuration);

            builder.Services.AddTransient<RateLimitMiddleware>();
            builder.Services.AddTransient<RateLimitDashboardMiddleware>();

            #endregion


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseIpRateLimiting();
            app.UseRouting();

            app.UseMiddleware<RateLimitMiddleware>();
            app.UseMiddleware<RateLimitDashboardMiddleware>();

            app.UseAuthorization();
            app.MapRazorPages();
            app.Run();
        }
    }
}