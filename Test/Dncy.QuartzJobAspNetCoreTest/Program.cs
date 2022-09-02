using Dncy.QuartzJob;
using Dncy.QuartzJob.AspNetCore;
using Dncy.QuartzJob.Model;
using Dncy.QuartzJob.Stores;

namespace Dncy.QuartzJobAspNetCoreTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient();

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddDncyQuartzJobCore();
            builder.Services.AddStaticJobDefined();
            builder.Services.AddJsonFileJobInfoStore();
            builder.Services.AddInMemoryLogStore();

            builder.Services.AddDncyQuartzJobWithUI();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseDncyQuartzJobUI();
            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();


            app.Run();
        }

    }
}