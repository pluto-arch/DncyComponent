using System.Diagnostics;
using System.Text.Json;
using Dncy.Diagnostics.Abstract;
using Dncy.Diagnostics.AspNetCore;

namespace DncyApmAspNetCore.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<DiagnosticContextAccessor>();
            builder.Services.AddSingleton<IDiagnosticListener, AspNetCoreDiagnosticListener>();
            builder.Services.AddSingleton<TraceDiagnsticListenerObserver>();
            var app = builder.Build();

            var logdd=app.Services.GetRequiredService<TraceDiagnsticListenerObserver>();
            app.Use(async (context, next) =>
            {
                var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
                var acc = context.RequestServices.GetRequiredService<DiagnosticContextAccessor>();
                await using var ext= new DiagnosticContext(context.RequestServices);
                acc.Context = ext;
                await next();
                var items = acc.Context.Items;
                await File.AppendAllTextAsync(Path.Combine(env.ContentRootPath,"wwwroot",$"{DateTime.Now:yyMMddHH}.txt"),JsonSerializer.Serialize(items));
            });
            DiagnosticListener.AllListeners.Subscribe(logdd);
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }

    public class TraceDiagnsticListenerObserver : IObserver<DiagnosticListener>
    {
        private IEnumerable<IDiagnosticListener> _listeners;

        public TraceDiagnsticListenerObserver(IEnumerable<IDiagnosticListener> listeners)
        {
            _listeners = listeners;
        }

        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {

        }

        public void OnNext(DiagnosticListener listener)
        {  
            var diagnosticListener = _listeners.FirstOrDefault(x => x.Name == listener.Name);
            if (diagnosticListener != null)
            {
                listener.Subscribe(diagnosticListener!);  
            }
        }
    }
}