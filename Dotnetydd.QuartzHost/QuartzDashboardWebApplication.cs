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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Dotnetydd.QuartzHost.Auth;
using Dotnetydd.QuartzHost.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Dotnetydd.QuartzHost;

public class QuartzDashboardWebApplication: IHostedService
{
    private readonly ILogger<QuartzDashboardWebApplication> _logger;

    private const string DashboardUrlDefaultValue = "http://localhost:18888";
    private const string DashboardUrlVariableName = "DOTNETYDD_QUARTZJOB_DASHBOARD_URL";


    private readonly WebApplication _app;

    private readonly bool _ishttps;

    public QuartzDashboardWebApplication(ILoggerFactory loggerFactory,Action<IServiceCollection> configureServices)
    {
        _logger = loggerFactory.CreateLogger<QuartzDashboardWebApplication>()??NullLogger<QuartzDashboardWebApplication>.Instance;

        var builder = WebApplication.CreateBuilder();
        var token=TokenGenerator.GenerateToken();
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(
            new Dictionary<string, string>
            {
                ["AppHost:Token"] = token
            }
        );
        var configuration= configurationBuilder.Build();
        builder.Services.AddSingleton<InnerIConfiguration>(s=>new InnerIConfiguration{InnerConfiguration = configuration});

        configureServices(builder.Services);
        loggerFactory.AddProvider(new BlazorConsoleLogProvider(cfg =>
        {
            cfg.LevelMap[LogLevel.Debug] = BlazorConsoleLogColor.Gray;
            cfg.LevelMap[LogLevel.Information] = BlazorConsoleLogColor.Green;
            cfg.LevelMap[LogLevel.Warning] = BlazorConsoleLogColor.Orange;
            cfg.LevelMap[LogLevel.Error] = BlazorConsoleLogColor.Red;
        }));

        builder.Services.AddSingleton<ILoggerFactory>(loggerFactory);
        
        var dashboardUris = EnvironmentHelper.GetAddressUris(DashboardUrlVariableName, DashboardUrlDefaultValue);

        if (dashboardUris.FirstOrDefault() is { } reportedDashboardUri)
        {
            _logger.LogInformation("Now listening on for dashboard: {dashboardUri}. access_token is {Token}", reportedDashboardUri.AbsoluteUri.TrimEnd('/'),token);
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
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddCircuitOptions(options =>
            {
#if DEBUG
                options.DetailedErrors = true;
#endif
            });

        ConfigureAuthentication(builder);

        builder.Services.AddScoped<DataRepository>();
        builder.Services.AddSingleton<IJobInfoStore, InMemoryJobInfoStore>();
        
        builder.Services.AddMudServices();
        builder.Services.AddLocalization();
        builder.Services.AddHttpClient();
       

        builder.Services.AddHostedService<QuartzHostedService>();

        _app = builder.Build();
        

        _app.UseMiddleware<ValidateTokenMiddleware>();

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



        _app.UseAuthentication();
        _app.UseAuthorization();


        _app.UseAntiforgery();

        _app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        _app.MapPost("/api/validatetoken", async (string token, HttpContext httpContext) => await ValidateTokenMiddleware.TryAuthenticateAsync(token, httpContext).ConfigureAwait(false));

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


    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        var authentication = builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

        authentication.AddCookie(o=>
        {
            o.AccessDeniedPath="/AccessDenied";
            o.Cookie.Name= "dncy_quartz_dashboard_key";
            o.Cookie.HttpOnly=true;
            o.Cookie.SameSite=SameSiteMode.Strict;
            o.LoginPath="/login";
            o.ExpireTimeSpan=TimeSpan.FromDays(10);
        });

        builder.Services.AddAuthorization(options =>
        {
        });

    }


    public static async Task<bool> TryAuthenticateAsync(string incomingBrowserToken, HttpContext httpContext)
    {
        if (string.IsNullOrEmpty(incomingBrowserToken))
        {
            return false;
        }
        var claimsIdentity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "Local")],
            authenticationType: CookieAuthenticationDefaults.AuthenticationScheme);
        var claims = new ClaimsPrincipal(claimsIdentity);

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims).ConfigureAwait(false);
        return true;
    }



    private static bool IsHttps(Uri uri) => string.Equals(uri.Scheme, "https", StringComparison.Ordinal);
}

public class InnerIConfiguration
{
    public IConfiguration InnerConfiguration { get; set; }
}