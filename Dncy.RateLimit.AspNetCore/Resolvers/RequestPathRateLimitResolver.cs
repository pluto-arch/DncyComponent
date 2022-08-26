using System.Text.RegularExpressions;
using Dncy.RateLimit.AspNetCore.Options;
using Dncy.RateLimit.Core;
using Dncy.RateLimit.Core.Algorithms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Dncy.RateLimit.AspNetCore;

public class RequestPathRateLimitResolver : IRateLimitTargetResolver
{
    private readonly IEnumerable<ILimitAlgorithm> _algorithms;
    private readonly RequestPath[] _limitConfigs;


    public RequestPathRateLimitResolver(IEnumerable<ILimitAlgorithm> algorithms, IOptions<LimitConfigurationOption> options)
    {
        _algorithms = algorithms;
        _limitConfigs = options?.Value?.RateLimit?.RequestPath;
    }

    public (string target, Dictionary<ILimitAlgorithm, List<RateLimitRule>> algoAndRules) ResolveWithRule(HttpContext context)
    {
        if (_algorithms == null || !_algorithms.Any() || _algorithms.All(x => !x.Avaliable))
        {
            return default;
        }

        if (!LimitConfigsIsValid())
        {
            return default;
        }

        var requestPath = context.Request.Path;
        var method = context.Request.Method;

        var target = $"{method}:{requestPath}";
        var rules = _limitConfigs.Where(x => x.Target == target);
        if (!rules.Any())
        {
            return default;
        }

        var res = new Dictionary<ILimitAlgorithm, List<RateLimitRule>>();
        foreach (var item in rules)
        {
            var alo = _algorithms.FirstOrDefault(x => x.Name == item.Alog && x.Avaliable);
            if (alo == null)
            {
                continue;
            }

            if (res.ContainsKey(alo))
            {
                res[alo].AddRange(alo.Parse(item.Rule));
            }
            else
            {
                res[alo] = alo.Parse(item.Rule).ToList();
            }
        }

        return (target, res);
    }


    bool LimitConfigsIsValid()
    {
        if (_limitConfigs == null || !_limitConfigs.Any())
        {
            return false;
        }

        return true;
    }
}