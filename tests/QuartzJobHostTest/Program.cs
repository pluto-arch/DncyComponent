using QuartzJobHostTest;
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
    });
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
