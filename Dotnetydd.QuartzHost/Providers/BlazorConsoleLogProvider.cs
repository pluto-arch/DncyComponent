using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using Dotnetydd.QuartzHost.Models;
using Dotnetydd.QuartzHost.Storage;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

namespace Dotnetydd.QuartzHost.Providers;


[ProviderAlias("BlazorConsoleLogger")]
public class BlazorConsoleLogProvider : ILoggerProvider
{
    private readonly BlazorConsoleLoggerConfig _config;
    public BlazorConsoleLogProvider(Action<BlazorConsoleLoggerConfig> config)
    {
        _config = new BlazorConsoleLoggerConfig();
        config?.Invoke(_config);
    }


    public ILogger CreateLogger(string categoryName)
    {
        return new BlazorConsoleLogger(_config);
    }

    public void Dispose()
    {
        return;
    }
}

public class BlazorConsoleLoggerConfig
{
    public Dictionary<LogLevel, BlazorConsoleLogColor> LevelMap { get; set; } = new();
}


public enum BlazorConsoleLogColor
{
    Red,
    Green,
    Blue,
    Orange,
    Gray,
    Black,
    White
}


public class BlazorConsoleLogger : ILogger
{
    private readonly BlazorConsoleLoggerConfig _levelConfig;
    private readonly object _lock = new();

    private static ConcurrentQueue<string> _buffer= new();

    public BlazorConsoleLogger(BlazorConsoleLoggerConfig levelConfig)
    {
        _levelConfig = levelConfig;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => default;

    public bool IsEnabled(LogLevel logLevel)=>_levelConfig.LevelMap.ContainsKey(logLevel);


    private static readonly StringBuilder sb = new ();


    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        try
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            Console.WriteLine(logLevel.ToString());
            _ = _levelConfig.LevelMap.TryGetValue(logLevel, out var value);
            var stateType = state.GetType();
            var msg = formatter(state, exception);
            var exeptionMsg=exception?.Message;
            if (!string.IsNullOrEmpty(exeptionMsg)&&!string.IsNullOrWhiteSpace(exeptionMsg))
            {
                exeptionMsg = $"<br/> {exeptionMsg}";
            }

            var color = _colorMap[BlazorConsoleLogColor.Gray];
            _ = _colorMap.TryGetValue(value,out color);
            sb.AppendLine($"<span>[{DateTimeOffset.Now:yy-MM-dd HH:mm:ss.fff}] <b style='color:{color}'> [{logLevel}] </b> {msg} <br/> {exeptionMsg} - [{stateType?.FullName}] </span>");
            lock (_lock)
            {
                _buffer.Enqueue(sb.ToString());
            }
            RaiseSubscriptionChanged(AppSubscribe.LoggerSubscriptions);
        }
        catch
        {
            // ignored
        }
    }





    private void RaiseSubscriptionChanged(List<Subscription> subscriptions)
    {
        lock (_lock)
        {
            var data = DequeueMultiple(20);
            var dataString = $@"<p>{string.Join("",data)}</p>";
            foreach (var subscription in subscriptions)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await subscription.ExecuteAsync(dataString).ConfigureAwait(false);
                    }
                    catch 
                    {
                        // ignored
                    }
                });
            }
        }
    }

    List<string> DequeueMultiple(int count)
    {
        var items = new List<string>();

        for (int i = 0; i < count; i++)
        {
            if (_buffer.TryDequeue(out var item))
            {
                items.Add(item);
            }
            else
            {
                break;
            }
        }

        return items;
    }

    static readonly Dictionary<BlazorConsoleLogColor,string> _colorMap= new()
    {
        {BlazorConsoleLogColor.Red,"red"},
        {BlazorConsoleLogColor.Green,"green"},
        {BlazorConsoleLogColor.Blue,"blue"},
        {BlazorConsoleLogColor.Orange,"orange"},
        {BlazorConsoleLogColor.Gray,"gray"},
        {BlazorConsoleLogColor.Black,"black"},
        {BlazorConsoleLogColor.White,"white"}
    };
}