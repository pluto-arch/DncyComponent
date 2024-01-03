using Dotnetydd.MultiTenancy;
using Dotnetydd.MultiTenancy.AspNetCore;
using Dotnetydd.MultiTenancy.AspNetCore.TenantIdentityParse;
using Dotnetydd.MultiTenancy.Store;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpContextAccessor();

var config = builder.Configuration;


builder.Services.Configure<TenantConfigurationOptions>(config); // read from configuration
builder.Services.AddTransient<ITenantStore, DefaultTenantStore>(); // default use TenantConfigurationOptions
builder.Services.AddTransient<IConnectionStringResolver, DefaultConnectionStringResolver>(); // read connection string from configuration

builder.Services.AddSingleton<ICurrentTenantAccessor, CurrentTenantAccessor>();
builder.Services.AddTransient<ICurrentTenant, CurrentTenant>();
builder.Services.AddTransient<ITenantResolver, TenantResolver>();
builder.Services.AddTransient<ITenantIdentityParse, HeaderTenantIdentityParse>(x=>new HeaderTenantIdentityParse(headerDic =>
{
    if (headerDic.ContainsKey("tenant"))
    {
        return headerDic["tenant"];
    }

    return null;
})); // parse tenant from http header
builder.Services.AddTransient<MultiTenancyMiddleware>(); // tenant middleware






var app = builder.Build();
app.UseMiddleware<MultiTenancyMiddleware>();
app.MapGet("/", () => "Hello World!");

app.Map("/tenant",(ICurrentTenant tenant) =>
{
    var ct=tenant?.Name;
    return Results.Ok(ct);
});

app.Run();
