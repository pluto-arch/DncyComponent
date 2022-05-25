# AspNetCore multi-tenancy 模块

多租户模块是可自定义的，也具有默认的功能实现。

> 默认的租户数据是使用配置文件。

## 默认
```csharp
builder.Services.Configure<TenantConfigurationOptions>(config); // 从配置文件中取租户数据 这是默认
builder.Services.AddSingleton<ICurrentTenantAccessor, CurrentTenantAccessor>();
builder.Services.AddTransient<ICurrentTenant, CurrentTenant>();

builder.Services.AddTransient<IConnectionStringResolver, DefaultConnectionStringResolver>(); // 租户连接字符串解析器 默认是从配置文件中租户信息中获取
builder.Services.AddTransient<ITenantStore, DefaultTenantStore>(); // 租户存储器 默认是从配置文件中租户信息中获取
builder.Services.AddTransient<ITenantResolver, TenantResolver>(); // 租户解析器 再中间件中解析租户，需要配合ITenantConstruct
builder.Services.AddTransient<ITenantConstruct, HeaderTenantConstruct>(x=>new HeaderTenantConstruct(headerDic =>
{
    if (headerDic.ContainsKey("tenant"))
    {
        return headerDic["tenant"];
    }

    return null;
})); // 租户构建器，默认实现了从http请求头或者域名的解析构建，根据需要可以自行实现。

builder.Services.AddTransient<MultiTenancyMiddleware>(); // 以服务方式注入租户中间件
```

### 租户解析器：

扩展方式：
1. 如果是http上下文中解析，请自行创建解析器，继承自`HttpTenantConstructBase`即可，然后再容器中注入。

```csharp
services.AddTransient<ITenantConstruct, CustomeTenantConstruct>();
```

2. 如果是其他方式，则自行实现 `ITenantConstruct` 接口即可

*可注入多个实现



## 租户中间件

正常使用 `MultiTenancyMiddleware`即可，如有其他规则，也可以可重写`MultiTenancyMiddleware`或者自定义。


注入后既可以使用 ICurrentTenant 进行访问当前对象