using Dotnetydd.QuartzJob;
using Dotnetydd.QuartzJob.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddDncyQuartzJobCore();
builder.Services.AddStaticJobDefined();
builder.Services.AddJsonFileJobInfoStore();
builder.Services.AddInMemoryLogStore();

builder.Services.AddQuartzJobWithDashboard(O =>
{
    O.HomePath = new PathString("/job");
    O.APIPath= new PathString("/job-api");
});



var app = builder.Build();
app.UseStaticFiles();
app.UseQuartzJobDashboard();

app.Run();
