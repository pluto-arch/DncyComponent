// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Dncy.PipelinePattern;
using Dncy.PipelinePatternTest;
using System.Net.WebSockets;
using RulesEngine.Extensions;
using RulesEngine.Models;
using Masuit.Tools.Hardware;

Console.WriteLine("Hello, World!");

var ser=new ServiceCollection();

ser.AddScoped<DemoService>();
ser.AddScoped<Rules>();

var ctx = new userCtx();
ctx.Properties.Add("hello", "world");

var pipeline=new AsyncPipelineBuilder(ser.BuildServiceProvider())
    .Use(async (context,sp, next) =>
    {
        var s=sp.GetService<Rules>();
        var uc = (userCtx) context;
        var ruleParam = new RuleParameter("Level",uc.Level);
        bool isOut=false;
        var dd = await s.rulesEngine().ExecuteAllRulesAsync("serveAlert",ruleParam);
        dd.OnFail(() =>
        {
            isOut=true;
            Console.WriteLine(string.Join(",",dd.Select(x => x.ActionResult?.Exception?.Message)));
        });
        if (!isOut)
        {
            await next();
        }
        else
        {
            Console.WriteLine("CPU使用率大于80");
        }
    })
    .Use(async (context,sp,  next) =>
    {
        await next();
    })
    .Use(async (context,sp,  next) =>
    {
        await next();
    })
    .Use(async (context,sp,  next) =>
    {
        await next();
    })
    .Use(async (context,sp,  next) =>
    {
        await next();
    })
    .Use<demoMiddleware>()
    .Use(async (context,sp,  next) =>
    {
        await next();
    })
    .Build();

foreach (var item in Enumerable.Range(1,200))
{
    ctx.Level=SystemInfo.CpuLoad;
    await pipeline.Invoke(ctx);
    await Task.Delay(1000);
}

Console.Read();



class userCtx : DataContext
{
    public float Level { get; set; }

    public int Score { get; set; }
}

class DemoService
{
    public void DoFilter(DataContext data)
    {
        Console.WriteLine(data.Properties["hello"]);
    }
}


class demoMiddleware : IPipelineMiddleware
{
    /// <inheritdoc />
    public async Task InvokeAsync(DataContext context,IServiceProvider service, AsyncRequestDelegate next)
    {
        var uc = (userCtx) context;
        Console.WriteLine($"demoMiddleware :{uc.Level} ");
        await next(context);
    }
}