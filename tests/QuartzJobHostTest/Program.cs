using Dotnetydd.QuartzHost;
using QuartzJobHostTest;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHostedService<DemoHostService>();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
