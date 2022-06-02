using Dncy.MultiTenancy;
using Dncy.MultiTenancy.AspNetCore;
using Dncy.MultiTenancy.ConnectionStrings;
using Dncy.MultiTenancy.Store;
using Dncy.MultiTenancy.Test;
using Dncy.Permission;
using Dncy.Permission.UnitTest.Definitions;
using Microsoft.AspNetCore.Authorization;

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
builder.Services.AddTransient<ITenantIdentityParse, HeaderTenantIdentityParse>(x=>new HeaderTenantIdentityParse(headerDic =>
{
    if (headerDic.ContainsKey("tenant"))
    {
        return headerDic["tenant"];
    }

    return null;
}));
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionRequirementHandler>();
builder.Services.AddTransient<MultiTenancyMiddleware>();


#region permission

builder.Services.AddScoped<IPermissionChecker, DefaultPermissionChecker>();
            
// permission definition 
builder.Services.AddSingleton<IPermissionDefinitionManager, DefaultPermissionDefinitionManager>();
builder.Services.AddSingleton<IPermissionDefinitionProvider, ProductPermissionDefinitionProvider>();
builder.Services.AddTransient<IPermissionGrantStore, InMemoryPermissionGrantStore>();
builder.Services.AddTransient<IPermissionManager, InMemoryPermissionManager>(); 
builder.Services.AddTransient<IPermissionValueProvider, RolePermissionValueProvider>();
builder.Services.AddTransient<IPermissionValueProvider, UserPermissionValueProvider>();
#endregion

builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<MultiTenancyMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
