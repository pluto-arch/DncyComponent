using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dncy.MQMessageActivator
{
    public class MessageHandlerActivator
    {
        private readonly IServiceScopeFactory _scopeFactory;

#if NETCOREAPP3_1
        private static readonly Lazy<ConcurrentBag<SubscribeDescriptor>> _lazySubscribes = new Lazy<ConcurrentBag<SubscribeDescriptor>>(EnsureSubscribeDescriptorsInitialized, true);
#else
        private static readonly Lazy<ConcurrentBag<SubscribeDescriptor>> _lazySubscribes = new(EnsureSubscribeDescriptorsInitialized, true);
#endif

        private static readonly Lazy<ConcurrentDictionary<Type, ObjectFactory>> _lazyCacheObjFactory = new Lazy<ConcurrentDictionary<Type, ObjectFactory>>(() => new ConcurrentDictionary<Type, ObjectFactory>(), true);

        public MessageHandlerActivator(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }


        public async Task ProcessRequestAsync(string route, string message)
        {
           
            using (var sc = _scopeFactory.CreateScope())
            {
                var logger=sc.ServiceProvider.GetRequiredService<ILogger<MessageHandlerActivator>>();
                logger.LogDebug("receive message：{msg}. on route：{route}。",message,route);
                foreach (SubscribeDescriptor subscribeDescriptor in _lazySubscribes.Value)
                {
#if NETCOREAPP3_1
                    RouteValueDictionary matchedRouteValues = new RouteValueDictionary();
#else
                    RouteValueDictionary matchedRouteValues = new();
#endif
                   
                    if (RouteMatcher.TryMatch(subscribeDescriptor.AttributeRouteInfo.Template, route, matchedRouteValues))
                    {
                        var parameterValues = new List<object>();

                        var valueProviders = new Dictionary<string, object?>(matchedRouteValues, StringComparer.OrdinalIgnoreCase) { { string.Empty, message } };

                        foreach (var parameterInfo in subscribeDescriptor.Parameters)
                        {
                            parameterValues.Add(await BindModelAsync(parameterInfo, valueProviders));
                        }


                        var instanceType = subscribeDescriptor.MethodInfo.DeclaringType;

                        if (instanceType != null)
                        {
                            var createFactory = CreateOrCacheObjectFactory(instanceType);
                            if (createFactory == null)
                            {
                                throw new InvalidOperationException($"unable create {instanceType.Name} type");
                            }
                            var handler = (MessageHandler)createFactory(sc.ServiceProvider, arguments: null);
                            handler.Context = new MQMessageContext { OriginalMessage = message };
                            if (subscribeDescriptor.MethodInfo.ReturnType.IsAssignableTo(typeof(IAsyncResult)))
                            {
                                var task = (Task?)subscribeDescriptor.MethodInfo.Invoke(handler, parameterValues.ToArray());
                                task ??= Task.CompletedTask;
                                await task;
                                continue;
                            }
                            subscribeDescriptor.MethodInfo.Invoke(handler, parameterValues.ToArray());
                        }
                    }
                }
            }
        }


        private static ObjectFactory? CreateOrCacheObjectFactory(Type instanceType)
        {
            ObjectFactory? createFactory;
            if (_lazyCacheObjFactory.Value.ContainsKey(instanceType))
            {
                createFactory = _lazyCacheObjFactory.Value[instanceType];
            }
            else
            {
                createFactory = ActivatorUtilities.CreateFactory(instanceType, Type.EmptyTypes);
                if (createFactory != null)
                {
                    _lazyCacheObjFactory.Value.TryAdd(instanceType, createFactory);
                }
            }

            return createFactory;
        }


        private Task<object?> BindModelAsync(ParameterInfo parameterInfo, Dictionary<string, object?> valueProviders)
        {
            object? parameterValue = parameterInfo.ParameterType.GetDefaultValue();

            try
            {
                if (parameterInfo.Name != null && valueProviders.TryGetValue(parameterInfo.Name, out object? value))
                {
                    parameterValue = Convert.ChangeType(value, parameterInfo.ParameterType);
                }
                else if (parameterInfo.ParameterType != typeof(object) && Type.GetTypeCode(parameterInfo.ParameterType) == TypeCode.Object)
                {
                    var modelValue = valueProviders[string.Empty]?.ToString();
                    parameterValue ??= modelValue != null ? JsonSerializer.Deserialize(modelValue, parameterInfo.ParameterType) : parameterValue;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Binding parameters {parameterInfo.Name} failed", e);
            }

            return Task.FromResult(parameterValue);
        }



        private static ConcurrentBag<SubscribeDescriptor> EnsureSubscribeDescriptorsInitialized()
        {
#if NETCOREAPP3_1
            ConcurrentBag<SubscribeDescriptor> subscribeDescriptors = new ConcurrentBag<SubscribeDescriptor>();
#else
            ConcurrentBag<SubscribeDescriptor> subscribeDescriptors = new();
#endif
            

            var exportedTypes = AppDomain.CurrentDomain.GetAssemblies().Where(e => !e.IsDynamic).SelectMany(e => e.ExportedTypes);

            var handlerImplementationTypes = exportedTypes.Where(t => t.IsAssignableTo(typeof(MessageHandler)) && t.IsClass);

            foreach (var implementationType in handlerImplementationTypes)
            {
                var methodInfos = implementationType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (var methodInfo in methodInfos)
                {
                    SubscribeAttribute? subscribeAttribute = methodInfo.GetCustomAttribute<SubscribeAttribute>();
                    if (subscribeAttribute != null)
                    {
#if NETCOREAPP3_1
                        SubscribeDescriptor subscribeDescriptor = new SubscribeDescriptor()
                        {
                            AttributeRouteInfo = subscribeAttribute,
                            MethodInfo = methodInfo,
                            Parameters = methodInfo.GetParameters()
                        };
#else
                        SubscribeDescriptor subscribeDescriptor = new()
                        {
                            AttributeRouteInfo = subscribeAttribute,
                            MethodInfo = methodInfo,
                            Parameters = methodInfo.GetParameters()
                        };
#endif

                        subscribeDescriptors.Add(subscribeDescriptor);
                    }
                }
            }

            return subscribeDescriptors;
        }
    }
}

