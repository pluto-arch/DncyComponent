# multi-tenancy 模块

多租户模块是可自定义的，也具有默认的功能实现。

> 默认的租户数据是使用配置文件。

## 默认
services.Configure<TenantConfigurationOptions>(configuration); // 从配置文件中读取所有租户数据
services.AddTransient<IConnectionStringResolver, DefaultConnectionStringResolver>(); // 租户数据库连接字符串解析器，默认是从配置文件中解析
services.AddTransient<ITenantStore, DefaultTenantStore>(); // 租户store，只读。默认是配置文件，自定义可设置从其他持久化设备读取。这里读取后的仅仅是系统运行时的租户信息，和数据库或者持久化地方的没有关系，但是需要用他们进行初始化和检查。



注入后既可以使用 ICurrentTenant 进行访问当前对象


## 配置文件中的租户配置示例
```json
{
	"Tenants": [
    {
      "TenantId": "T20210602000001",
      "TenantName": "tenant1",
      "ConnectionStrings": {
        "Default":
          "Server=127.0.0.1,1433;Database=Pnct_T20210602000001;User Id=sa;Password=970307lBX;Trusted_Connection = False;"
      }
    },
    {
      "TenantId": "T20210602000002",
      "TenantName": "tenant2",
      "ConnectionStrings": {
        "Default":
          "Server=127.0.0.1,1433;Database=Pnct_T20210602000002;User Id=sa;Password=970307lBX;Trusted_Connection = False;"
      }
    }
  ]
}
```


asp.net core中使用请参见`Dncy.MultiTenancy.AspNetCore`
