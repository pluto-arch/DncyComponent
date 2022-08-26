namespace Dncy.RateLimit.Core.Algorithms
{
    public interface ILimitAlgorithm
    {
        bool Avaliable { get; set; }

        string Name { get; }

        LimitCheckResult  Check(LimitCheckContext context,List<RateLimitRule> rules);


        IEnumerable<RateLimitRule> Parse(string[] itemRule);
    }
}

