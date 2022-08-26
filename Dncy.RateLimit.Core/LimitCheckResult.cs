namespace Dncy.RateLimit.Core;

public struct LimitCheckResult
{
    public bool Passed { get; set; }

    public uint Count { get; set; }

    public TimeSpan PeriodTime { get; set; }

    public uint TotalCount { get; set; }

    public TimeSpan StatWindow { get; set; }
}