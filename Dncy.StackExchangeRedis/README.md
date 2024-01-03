# 介绍
使用 `StackExchange.Redis` 封装的redis操作库

# 使用方式

## NET CORE

- IOC中注入RedicClient对象：

```csharp
builder.Services.AddRedisClient(o =>
{
    o.CommandMap = CommandMap.Default;
    o.DefaultDatabase = 0;
    o.ClientName = "docker01";
    o.Password = "";
    o.KeepAlive = 180;
    o.EndPoints.Add("localhost",6379);
});

builder.Services.AddRedisClient(o =>
{
    o.CommandMap = CommandMap.Default;
    o.ClientName = "docker02";
    o.DefaultDatabase = 1;
    o.Password = "";
    o.KeepAlive = 180;
    o.EndPoints.Add("localhost",6379);
});
// 注入工厂，可以解析多个client对象。由ClientName区分
builder.Services.AddRedisClientFactory();
```
- 使用
```csharp
// 由工厂解析 需要 AddRedisClientFactory
app.MapGet("/users", async ([FromServices]RedisClientFactory redisClientFactory) =>
{
    var redis01 = redisClientFactory["docker01"];
    await redis01.Db.StringSetAsync("demoA","123123",TimeSpan.FromMinutes(3));
    return Results.Ok("demoA");

}).WithName("redsi01");

// 直接使用client
app.MapGet("/users", async ([FromServices]IRedisClient redis01) =>
{
    await redis01.Db.StringSetAsync("demoA","123123",TimeSpan.FromMinutes(3));
    return Results.Ok("demoA");

}).WithName("redsi01");
```

## NET Framework

```csharp
internal class Program
{
    private static Lazy<IRedisClient> Redis = new Lazy<IRedisClient>(InitRedis);
    private static IRedisClient InitRedis()
    {
        var opt = new ConfigurationOptions
        {
            CommandMap = CommandMap.Default,
            DefaultDatabase = 0,
            ClientName = "docker02",
            Password = "",
            KeepAlive = 180,
        };
        opt.EndPoints.Add("localhost", 6379);
        IRedisClient redis = new RedisClient(opt);
        return redis;
    }
    static async Task Main(string[] args)
    {
        var a =await  Redis.Value.Db.StringSetAsync("","",TimeSpan.Zero);
    }
}
```