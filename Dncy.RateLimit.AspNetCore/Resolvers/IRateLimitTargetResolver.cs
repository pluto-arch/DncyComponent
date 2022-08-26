using Dncy.RateLimit.Core;
using Dncy.RateLimit.Core.Algorithms;
using Microsoft.AspNetCore.Http;

namespace Dncy.RateLimit.AspNetCore;

public interface IRateLimitTargetResolver
{
    (string target,Dictionary<ILimitAlgorithm,List<RateLimitRule>> algoAndRules) ResolveWithRule(HttpContext context);
}