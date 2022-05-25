# AspNetCore multi-tenancy 模块

多租户模块是可自定义的，也具有默认的功能实现。

> 默认的租户数据是使用配置文件。

## 默认
services.Configure<TenantConfigurationOptions>(configuration); // 从配置文件中读取所有租户数据
services.AddTransient<IConnectionStringResolver, DefaultConnectionStringResolver>(); // 租户数据库连接字符串解析器，默认是从配置文件中解析
services.AddTransient<ITenantStore, DefaultTenantStore>(); // 租户store，只读。默认是配置文件，自定义可设置从其他持久化设备读取。这里读取后的仅仅是系统运行时的租户信息，和数据库或者持久化地方的没有关系，但是需要用他们进行初始化和检查。

### 租户解析器：
默认由从域名解析和从Http请求头解析。可自己扩展。
```csharp
services.AddTransient<ITenantConstruct, DomainNameTenantConstruct>(x=>new DomainNameTenantConstruct(hostString=> {
	// 这里解析hoststring，获取域名，然后根据自己规则解析
	return tenantIdOrName;
}));
}));
```
```

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