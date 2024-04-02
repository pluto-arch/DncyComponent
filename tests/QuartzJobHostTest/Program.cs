using QuartzJobHostTest;
using QuartzJobHostTest.Services;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddHostedService(_ =>
{
    var loggerFactory = LoggerFactory.Create(logBuilder =>
    {
        logBuilder.AddConsole();
    });
    return new DemoHostService(loggerFactory, service =>
    {
        service.AddScoped<DemoService>();
    });
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
