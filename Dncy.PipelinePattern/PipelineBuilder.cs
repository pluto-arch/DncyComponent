using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Dncy.PipelinePattern;


public delegate Task AsyncRequestDelegate(DataContext context);


public delegate void RequestDelegate(DataContext context);



public interface IAsyncPipelineBuilder
{
    IServiceProvider Service { get; }

    IAsyncPipelineBuilder Use(Func<DataContext, IServiceProvider, Func<Task>, Task> middleware);

    IAsyncPipelineBuilder Use<TMiddleware>() where TMiddleware : IPipelineMiddleware;

    AsyncRequestDelegate Build();
}



public class AsyncPipelineBuilder : IAsyncPipelineBuilder
{

    /// <inheritdoc />
    public IServiceProvider Service { get; }


    private readonly List<Func<AsyncRequestDelegate, AsyncRequestDelegate>> _middlewares = new List<Func<AsyncRequestDelegate, AsyncRequestDelegate>>();


    public AsyncPipelineBuilder(IServiceProvider serviceProvider)
    {
        Service = serviceProvider;
    }

    private IAsyncPipelineBuilder Use(Func<AsyncRequestDelegate, AsyncRequestDelegate> middleware)
    {
        _middlewares.Add(middleware);
        return this;
    }


    public IAsyncPipelineBuilder Use<TMiddleware>()
        where TMiddleware : IPipelineMiddleware
    {
        var of = CreateOrCacheObjectFactory(typeof(TMiddleware));
        var middleware = (TMiddleware)of.Invoke(Service, null);
        Use(next =>
        {
            return context => middleware.InvokeAsync(context, Service, next);
        });
        return this;
    }



    /// <inheritdoc />
    public IAsyncPipelineBuilder Use(Func<DataContext, IServiceProvider, Func<Task>, Task> middleware)
    {
        Use(next =>
        {
            return context => middleware(context, Service, () => next(context));
        });
        return this;
    }

    /// <inheritdoc />
    public AsyncRequestDelegate Build()
    {
        AsyncRequestDelegate app = context => Task.CompletedTask;
        var middlewares = _middlewares.ToImmutableArray().Reverse();
        foreach (var middleware in middlewares)
        {
            app = middleware(app);
        }
        return app;
    }



    private static readonly Lazy<ConcurrentDictionary<Type, ObjectFactory>> _lazyCacheObjFactory = new Lazy<ConcurrentDictionary<Type, ObjectFactory>>(() => new ConcurrentDictionary<Type, ObjectFactory>(), true);

    private static ObjectFactory CreateOrCacheObjectFactory(Type instanceType)
    {
        ObjectFactory createFactory;
        if (_lazyCacheObjFactory.Value.ContainsKey(instanceType))
        {
            createFactory = _lazyCacheObjFactory.Value[instanceType];
        }
        else
        {
            createFactory = ActivatorUtilities.CreateFactory(instanceType, Type.EmptyTypes);
            _lazyCacheObjFactory.Value.TryAdd(instanceType, createFactory);
        }

        return createFactory;
    }
}