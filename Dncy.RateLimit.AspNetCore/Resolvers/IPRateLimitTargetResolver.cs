using Dncy.RateLimit.Core;
using Dncy.RateLimit.Core.Algorithms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Dncy.RateLimit.AspNetCore;

public class IPRateLimitTargetResolver:IRateLimitTargetResolver
{
    /// <inheritdoc />
    public (string target, Dictionary<ILimitAlgorithm, List<RateLimitRule>> algoAndRules) ResolveWithRule(HttpContext context)
    {
        return (null, null);
    }
}