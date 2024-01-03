using Dotnetydd.Permission.Checker;
using Dotnetydd.Permission.Definition;
using Dotnetydd.Permission.Models;
using Dotnetydd.Permission.PermissionGrant;
using Dotnetydd.Permission.PermissionManager;
using Dotnetydd.Permission.ValueProvider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PermissionTest;
using PermissionTest.MyAuthentication;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// customer authentication
builder.Services.AddAuthentication(MyAuthenticationOptions.DefaultScheme)
    .AddScheme<MyAuthenticationOptions,MyAuthenticationHandler>(MyAuthenticationOptions.DefaultScheme,options => { });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionRequirementHandler>();

// permission checker
builder.Services.AddScoped<IPermissionChecker, DefaultPermissionChecker>();

// permission definition
builder.Services.AddSingleton<IPermissionDefinitionManager, DefaultPermissionDefinitionManager>();
builder.Services.AddSingleton<IPermissionDefinitionProvider, ProductPermissionDefinitionProvider>();

builder.Services.AddTransient<IPermissionGrantStore, InMemoryPermissionGrantStore>(); // store user permission in memory

builder.Services.AddTransient<IPermissionManager, InMemoryPermissionManager>(); // manage permission subject such as read from db and cached for checker

// customer permission value provider for checker
builder.Services.AddTransient<IPermissionValueProvider, RolePermissionValueProvider>();
builder.Services.AddTransient<IPermissionValueProvider, UserPermissionValueProvider>();

// IPermissionChecker --> List<IPermissionValueProvider> --> IPermissionManager

var app = builder.Build();

#region init demo permission for test user

var grantStore = app.Services.GetRequiredService<IPermissionGrantStore>();
// grant 'ProductPermission.Product.Default' permission for user with 'aaa' key
await grantStore.SaveAsync(ProductPermission.Product.Default, "User", MyAuthenticationOptions.UserId);

#endregion

app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => "Hello World!")
    .AllowAnonymous();

// need auth
app.MapGet("/product", async ([FromServices]IHttpContextAccessor http,[FromServices]IPermissionGrantStore store) =>
{
    var user=http.HttpContext?.User?.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier)?.Value;
    var permissions= await store.GetListAsync("User",user);
    return Results.Ok(new
    {
        User= user,
        Permissions=permissions
    });
}).RequireAuthorization(ProductPermission.Product.Default);

app.Run();


#region defined system permissions
public class ProductPermissionDefinitionProvider: IPermissionDefinitionProvider
{
    public void Define(PermissionDefinitionContext context)
    {
        var productGroup = context.AddGroup(ProductPermission.GroupName, "ProductPermission.ProductManager");
        PermissionDefinition userPermissionManager = productGroup.AddPermission(ProductPermission.Product.Default, "ProductPermission.ProductManager.Products");
        userPermissionManager.AddChild(ProductPermission.Product.Create, "ProductPermission.ProductManager.Products.Create");
        userPermissionManager.AddChild(ProductPermission.Product.Edit, "ProductPermission.ProductManager.Products.Edit");
        userPermissionManager.AddChild(ProductPermission.Product.Delete, "ProductPermission.ProductManager.Products.Delete");


        var rolePermissionManager = productGroup.AddPermission(ProductPermission.Device.Default, "ProductPermission.ProductManager.Device");
        rolePermissionManager.AddChild(ProductPermission.Device.Create, "ProductPermission.ProductManager.Device.Create");
        rolePermissionManager.AddChild(ProductPermission.Device.Edit, "ProductPermission.ProductManager.Device.Edit");
        rolePermissionManager.AddChild(ProductPermission.Device.Delete, "ProductPermission.ProductManager.Device.Delete");
    }
}

public class ProductPermission
{
    public const string GroupName = "ProductManager";


    public static class Product
    {
        public const string Default = GroupName + ".Products";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }


    public static class Device
    {
        public const string Default = GroupName + ".Devices";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
}
#endregion
