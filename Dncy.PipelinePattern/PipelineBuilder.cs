using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Dncy.PipelinePattern;


public delegate Task AsyncRequestDelegate(DataContext context);


public delegate void RequestDelegate(DataContext context);



public interface IAsyncPipelineBuilder
{
    IAsyncPipelineBuilder Use(Func<DataContext, Func<Task>, Task> middleware);

    IAsyncPipelineBuilder Use(IPipelineMiddleware middleware);

    AsyncRequestDelegate Build();
}



public class AsyncPipelineBuilder:IAsyncPipelineBuilder
{
    private readonly List<Func<AsyncRequestDelegate, AsyncRequestDelegate>> _middlewares = new List<Func<AsyncRequestDelegate, AsyncRequestDelegate>>();

    private IAsyncPipelineBuilder Use(Func<AsyncRequestDelegate, AsyncRequestDelegate> middleware)
    {
        _middlewares.Add(middleware);
        return this;
    }


    public IAsyncPipelineBuilder Use(IPipelineMiddleware middleware)
    {
        Use(next =>
        {
            return context =>
            {
                context.Increment();
                return middleware.InvokeAsync(context, next);
            };
        });
        return this;
    }

    /// <inheritdoc />
    public IAsyncPipelineBuilder Use(Func<DataContext, Func<Task>, Task> middleware)
    {
        Use(next =>
        {
            return context =>
            {
                context.Increment();
                return middleware(context, () => next(context));
            };
        });
        return this;
    }

    /// <inheritdoc />
    public AsyncRequestDelegate Build()
    {
        AsyncRequestDelegate app = context =>
        {
            context.Increment();
            return Task.CompletedTask;
        };
        var middlewares = _middlewares.ToImmutableArray().Reverse();
        foreach (var middleware in middlewares)
        {
            app = middleware(app);
        }
        return app;
    }
}