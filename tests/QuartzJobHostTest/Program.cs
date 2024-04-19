using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using QuartzJobHostTest;
using QuartzJobHostTest.Etl.DataSources;
using QuartzJobHostTest.Etl.Fetcher;
using QuartzJobHostTest.Services;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

//Log.Logger = new LoggerConfiguration()
//    .Enrich.FromLogContext()
//    .WriteTo.Console()
//    .CreateLogger();
builder.Services.AddHostedService(_ =>
{
    var loggerFactory = LoggerFactory.Create(logBuilder =>
    {
        //logBuilder.AddSerilog(dispose: true);
        logBuilder.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        });
    });
    return new DemoHostService(loggerFactory, service =>
    {
        service.AddScoped<DemoService>();

        service.AddSingleton<FetcherContextAccessor>();

        var assembly=Assembly.GetExecutingAssembly();

        var types = assembly.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDataSource<>)))
            .ToList();

        foreach (var item in types)
        {
            var entitTypies = item.GetInterfaces()
            .FirstOrDefault(x=>x.IsGenericType&&x.Name.StartsWith("IDataSource"))?.GetGenericArguments()[0];
            if (entitTypies != null)
            {
                var interfaceType = typeof(IDataSource<>).MakeGenericType(entitTypies);
                service.AddTransient(interfaceType,item);
            }
        }

    });
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
