using Dncy.MultiTenancy;
using Dncy.MultiTenancy.AspNetCore;
using Dncy.MultiTenancy.ConnectionStrings;
using Dncy.MultiTenancy.Store;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

var config = builder.Configuration;
builder.Services.Configure<TenantConfigurationOptions>(config);
builder.Services.AddSingleton<ICurrentTenantAccessor, CurrentTenantAccessor>();
builder.Services.AddTransient<ICurrentTenant, CurrentTenant>();
builder.Services.AddTransient<IConnectionStringResolver, DefaultConnectionStringResolver>();
builder.Services.AddTransient<ITenantStore, DefaultTenantStore>();
builder.Services.AddTransient<ITenantResolver, TenantResolver>();
builder.Services.AddTransient<ITenantConstruct, HeaderTenantConstruct>(x=>new HeaderTenantConstruct(headerDic =>
{
    if (headerDic.ContainsKey("tenant"))
    {
        return headerDic["tenant"];
    }

    return null;
}));

builder.Services.AddTransient<MultiTenancyMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<MultiTenancyMiddleware>();
app.MapControllers();

app.Run();
