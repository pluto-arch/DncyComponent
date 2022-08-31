using System.Configuration;
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

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddDncyQuartzJobCore();
            builder.Services.AddStaticJobs();
            builder.Services.AddSingleton<IJobInfoStore, JsonFileJobStore>();
            builder.Services.AddSingleton<IJobLogStore, InMemoryJobLog>();

            builder.Services.AddDncyQuartzJobWithUI();

            var app = builder.Build();
            InitJobsFromConfiguration(app.Services.GetRequiredService<IJobInfoStore>(),app.Configuration);
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

        /// <summary>
        /// 从配置文件中初始化静态job
        /// </summary>
        /// <param name="store"></param>
        private static void InitJobsFromConfiguration(IJobInfoStore store,IConfiguration configuration)
        {
            var jobs = configuration.GetSection("JobSettings")?.Get<List<JobSetting>>();
            if (jobs != null)
            {
                foreach (JobSetting job in jobs)
                {
                    if (!job.IsOpen)
                    {
                        continue;
                    }

                    store?.AddAsync(new JobInfoModel
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        TaskType = EnumTaskType.StaticExecute,
                        TaskName = job.Name,
                        DisplayName = job.DisplayName,
                        GroupName = job.GroupName,
                        Interval = job.Cron,
                        Describe = job.Description,
                        Status = EnumJobStates.Normal
                    });
                }
            }
        }
    }
}