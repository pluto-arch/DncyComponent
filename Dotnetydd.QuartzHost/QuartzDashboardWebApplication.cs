using Dotnetydd.QuartzHost.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using Dotnetydd.QuartzHost.Storage;
using MudBlazor.Services;
using Dotnetydd.QuartzHost.Providers;

namespace Dotnetydd.QuartzHost;

public class QuartzDashboardWebApplication: IHostedService
{
    private readonly ILogger<QuartzDashboardWebApplication> _logger;

    private const string DashboardUrlDefaultValue = "http://localhost:18888";
    private const string DashboardUrlVariableName = "DOTNETYDD_QUARTZJOB_DASHBOARD_URL";


    private readonly WebApplication _app;

    private readonly bool _ishttps;

    public QuartzDashboardWebApplication(ILogger<QuartzDashboardWebApplication> logger,Action<IServiceCollection> configureServices)
    {
        _logger = logger;

        var builder = WebApplication.CreateBuilder();
        builder.Host.ConfigureLogging(o =>
        {
            o.AddProvider(new BlazorConsoleLogProvider(cfg =>
            {
                cfg.LevelMap[LogLevel.Debug] = BlazorConsoleLogColor.Green;
                cfg.LevelMap[LogLevel.Information] = BlazorConsoleLogColor.White;
                cfg.LevelMap[LogLevel.Warning] = BlazorConsoleLogColor.Orange;
                cfg.LevelMap[LogLevel.Error] = BlazorConsoleLogColor.Red;
            }));
        });


        var dashboardUris = EnvironmentHelper.GetAddressUris(DashboardUrlVariableName, DashboardUrlDefaultValue);

        if (dashboardUris.FirstOrDefault() is { } reportedDashboardUri)
        {
            _logger.LogInformation("Now listening on for dashboard: {dashboardUri}", reportedDashboardUri.AbsoluteUri.TrimEnd('/'));
        }

        var dashboardHttpsPort = dashboardUris.FirstOrDefault(IsHttps)?.Port;

        _ishttps = dashboardHttpsPort is not null;

        builder.WebHost.ConfigureKestrel(kestrelOptions =>
        {
            ConfigureListenAddresses(kestrelOptions, dashboardUris);
        });


        if (!builder.Environment.IsDevelopment())
        {
            builder.WebHost.UseStaticWebAssets();
        }


        if (_ishttps)
        {
            builder.Services.Configure<HttpsRedirectionOptions>(options => options.HttpsPort = dashboardHttpsPort);
        }

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddScoped<DataRepository>();
        builder.Services.AddSingleton<IJobInfoStore, InMemoryJobInfoStore>();
        
        builder.Services.AddMudServices();
        builder.Services.AddLocalization();



        builder.Services.AddHttpClient();
        configureServices(builder.Services);

        builder.Services.AddHostedService<QuartzHostedService>();

        _app = builder.Build();

        if (!_app.Environment.IsDevelopment())
        {
            _app.UseExceptionHandler("/Error");
        }

        if (_ishttps)
        {
            _app.UseHttpsRedirection();
        }

        _app.UseStaticFiles(new StaticFileOptions()
        {
            OnPrepareResponse = (context) =>
            {
                if (context.Context.Response.Headers.CacheControl.Count == 0)
                {
                    context.Context.Response.Headers.CacheControl = "no-cache";
                }
            }
        });

        _app.UseAntiforgery();

        _app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _app.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _app.StopAsync(cancellationToken).ConfigureAwait(false);
    }


    private static void ConfigureListenAddresses(KestrelServerOptions kestrelOptions, Uri[] uris, HttpProtocols? httpProtocols = null)
    {
        foreach (var uri in uris)
        {
            if (uri.IsLoopback)
            {
                kestrelOptions.ListenLocalhost(uri.Port, options =>
                {
                    ConfigureListenOptions(options, uri, httpProtocols);
                });
            }
            else
            {
                kestrelOptions.Listen(IPAddress.Parse(uri.Host), uri.Port, options =>
                {
                    ConfigureListenOptions(options, uri, httpProtocols);
                });
            }
        }

        static void ConfigureListenOptions(ListenOptions options, Uri uri, HttpProtocols? httpProtocols)
        {
            if (IsHttps(uri))
            {
                options.UseHttps();
            }
            if (httpProtocols is not null)
            {
                options.Protocols = httpProtocols.Value;
            }
        }
    }



    private static bool IsHttps(Uri uri) => string.Equals(uri.Scheme, "https", StringComparison.Ordinal);
}