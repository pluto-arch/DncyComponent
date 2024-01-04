using Dotnetydd.QuartzHost.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Net;
using Dotnetydd.QuartzHost.Storage;

namespace Dotnetydd.QuartzHost;

public class QuartzDashboardWebApplication: IHostedService
{
    private readonly ILogger<QuartzDashboardWebApplication> _logger;

    private const string DashboardUrlDefaultValue = "http://localhost:18888";
    private const string DashboardUrlVariableName = "DOTNETYDD_QUARTZJOB_DASHBOARD_URL";


    private readonly WebApplication _app;

    private readonly bool _ishttps;

    public QuartzDashboardWebApplication(ILogger<QuartzDashboardWebApplication> logger)
    {
        _logger = logger;

        var builder = WebApplication.CreateBuilder();
        builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.None);
        builder.Logging.AddFilter("Microsoft.AspNetCore.Server.Kestrel", LogLevel.Error);


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
            // This is set up automatically by the DefaultBuilder when IsDevelopment is true
            // But since this gets packaged up and used in another app, we need it to look for
            // static assets on disk as if it were at development time even when it is not
            builder.WebHost.UseStaticWebAssets();
        }


        if (_ishttps)
        {
            // Explicitly configure the HTTPS redirect port as we're possibly listening on multiple HTTPS addresses
            // if the dashboard OTLP URL is configured to use HTTPS too
            builder.Services.Configure<HttpsRedirectionOptions>(options => options.HttpsPort = dashboardHttpsPort);
        }

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddSingleton<DataRepository>();
        builder.Services.AddSingleton<IJobInfoStore, InMemoryJobInfoStore>();
        
        builder.Services.AddFluentUIComponents();
        builder.Services.AddLocalization();

        _app = builder.Build();

        // Configure the HTTP request pipeline.
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
                // If Cache-Control isn't already set to something, set it to 'no-cache' so that the
                // ETag and Last-Modified headers will be respected by the browser.
                // This may be able to be removed if https://github.com/dotnet/aspnetcore/issues/44153
                // is fixed to make this the default
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