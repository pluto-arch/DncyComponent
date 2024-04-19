namespace Dotnetydd.QuartzHost.Models;

public sealed class Subscription : IDisposable
{
    private readonly Func<SubscribeCallbackContext,Task> _callback;
    private readonly Action _unsubscribe;

    public string? ApplicationId { get; }
    public SubscriptionType SubscriptionType { get; }

    public Subscription(string applicationId, SubscriptionType subscriptionType, Func<SubscribeCallbackContext,Task> callback, Action unsubscribe)
    {
        ApplicationId = applicationId;
        SubscriptionType = subscriptionType;
        _callback = callback;
        _unsubscribe = unsubscribe;
    }

    public Task ExecuteAsync() => _callback(null);

    public Task ExecuteAsync<T>(T data) => _callback(new SubscribeCallbackContext(){Data = data});

    public void Dispose()
    {
        _unsubscribe();
    }
}


public sealed class SubscribeCallbackContext
{
    public object Data { get; set; }

    public static SubscribeCallbackContext Default => new SubscribeCallbackContext();
}

/// <summary>
/// Indicates the purpose of the subscription.
/// </summary>
public enum SubscriptionType
{
    /// <summary>
    /// On subscription notification the app will read the latest data.
    /// </summary>
    Read,
    /// <summary>
    /// On subscription notification the app won't read the latest data.
    /// </summary>
    Other
}