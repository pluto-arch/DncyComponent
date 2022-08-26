// See https://aka.ms/new-console-template for more information

using Dncy.PipelinePattern;

Console.WriteLine("Hello, World!");




var ctx = new userCtx();

var pipeline=new AsyncPipelineBuilder()
    .Use(async (context, next) =>
    {
        Console.WriteLine("1");
        await next();
    })
    .Use(async (context, next) =>
    {
        Console.WriteLine("2");
        await next();
    })
    .Use(async (context, next) =>
    {
        Console.WriteLine("3");
        await next();
    })
    .Use(async (context, next) =>
    {
        Console.WriteLine("4");
        await next();
    })
    .Use(async (context, next) =>
    {
        Console.WriteLine("5");
        await next();
    })
    .Use(new demoMiddleware())
    .Use(async (context, next) =>
    {
        Console.WriteLine("6");
        await next();
    })
    .Build();

await pipeline.Invoke(ctx);

Console.WriteLine("经理管道个数：{0}",ctx.Count);
Console.Read();



class userCtx : DataContext
{
    public int Level { get; private set; }

    public int Score { get; set; }
}


class demoMiddleware : IPipelineMiddleware
{
    /// <inheritdoc />
    public async Task InvokeAsync(DataContext context, AsyncRequestDelegate next)
    {
        Console.WriteLine("demoMiddleware");
        await next(context);
    }
}