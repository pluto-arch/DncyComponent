namespace Dncy.RateLimit.Core.Counter;

public class FixedWindowCounter
{
    /// <summary>
    /// The Count Value
    /// </summary>
    /// <value></value>
    public uint Value { get; set; }

    /// <summary>
    /// The start time of current window 
    /// </summary>
    /// <value></value>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// The statistical time window
    /// </summary>
    /// <value></value>
    public TimeSpan StatWindow { get; set; }
}