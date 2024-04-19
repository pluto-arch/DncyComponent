using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Dotnetydd.QuartzHost.Models;
using Dotnetydd.QuartzHost.Storage;
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
        return new BlazorConsoleLogger(_config, categoryName);
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


public partial class BlazorConsoleLogger : ILogger
{
    private readonly BlazorConsoleLoggerConfig _levelConfig;
    private readonly string _categoryName;
    private readonly object _lock = new();

    private static ConcurrentQueue<string> _buffer = new();

    public BlazorConsoleLogger(BlazorConsoleLoggerConfig levelConfig, string categoryName)
    {
        _levelConfig = levelConfig;
        _categoryName = categoryName;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => default;

    public bool IsEnabled(LogLevel logLevel) => _levelConfig.LevelMap.ContainsKey(logLevel);


    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        try
        {
            //if (!IsEnabled(logLevel))
            //{
            //    return;
            //}
            _ = _levelConfig.LevelMap.TryGetValue(logLevel, out var value);
            var message = $"<div>{Formatter(state)}</div>";
            if (exception is not null)
            {
                message = $"<br/> <div style='color:red'> {exception.Message} <br/> {exception.StackTrace}</div>";
            }
            _ = _colorMap.TryGetValue(value, out var color);
            
            RaiseSubscriptionChanged(AppSubscribe.LoggerSubscriptions, 
                $"<section> <span>[{DateTimeOffset.Now:MM-dd HH:mm:ss.fff}] </span> <b>  [<span style='color:{color}'>{logLevel.ToString()[..4]} </span>:{_categoryName}] </b> {message}  </section>");
        }
        catch
        {
            // ignored
        }
    }

    const string OriginalFormatPropertyName = "{OriginalFormat}";
    static Regex regex = new Regex(@"\{.*?\}");
    private string Formatter<TState>(TState state)
    {
        string temp = "";
        if (state is IEnumerable<KeyValuePair<string, object>> structure)
        {
            var containsTemplate = structure.Any(x => x is { Key: OriginalFormatPropertyName, Value: string});

            if (containsTemplate)
            {
                temp = (structure.FirstOrDefault(x => x is { Key: OriginalFormatPropertyName, Value: string}).Value as string)??"";

                var templateProperties = LogTemplatePropertyRegex().Matches(temp.ToString());

                if (templateProperties is {Count:>0})
                {
                    foreach (Match tp in templateProperties)
                    {
                        var value = structure.FirstOrDefault(x => x.Key == tp.Value.Trim('{').Trim('}')).Value;
                        if (tp.Value.Contains('@')&&value is not null)
                        {
                            temp = temp.Replace(tp.Value, JsonSerializer.Serialize(value));
                        }
                        else
                        {
                            temp = temp.Replace(tp.Value, value?.ToString()??"");
                        }
                    }
                }

            }
        }
        return temp;
    }


    [GeneratedRegex( @"\{.*?\}", RegexOptions.IgnorePatternWhitespace|RegexOptions.Compiled)]
    private static partial Regex LogTemplatePropertyRegex();


    static string ReplaceWith(Span<char> input, char oldChar, char newChar)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == oldChar)
            {
                input[i] = newChar;
            }
        }
        return new string(input);
    }


    private void RaiseSubscriptionChanged(List<Subscription> subscriptions, string value)
    {
        lock (_lock)
        {
            foreach (var subscription in subscriptions)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await subscription.ExecuteAsync(value).ConfigureAwait(false);
                    }
                    catch
                    {
                        // ignored
                    }
                });
            }
        }
    }

    static readonly Dictionary<BlazorConsoleLogColor, string> _colorMap = new()
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
