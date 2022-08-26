namespace Dncy.RateLimit.Core.Rules;

public class FixedWindowRule:RateLimitRule
{
    public TimeSpan WindowTime { get; set; }

    public uint LimitNumber { get; set; }

    public override uint GetLimitThreshold()
    {
        return LimitNumber;
    }
}
