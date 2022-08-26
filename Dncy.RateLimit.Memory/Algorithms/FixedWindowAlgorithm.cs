using System.Collections.Concurrent;
using Dncy.RateLimit.Core;
using Dncy.RateLimit.Core.Algorithms;
using Dncy.RateLimit.Core.Counter;
using Dncy.RateLimit.Core.Rules;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Dncy.RateLimit.MemoryCache.Algorithms
{
    /// <summary>
    /// 固定窗口算法
    /// </summary>
    public class FixedWindowAlgorithm : ILimitAlgorithm
    {
        private readonly ConcurrentDictionary<string, FixedWindowCounter> _cache = new ConcurrentDictionary<string, FixedWindowCounter>();


        public FixedWindowAlgorithm()
        {
            Avaliable=true;
        }


        /// <inheritdoc />
        public bool Avaliable { get; set; }

        /// <inheritdoc />
        public string Name => "FW";

        /// <inheritdoc />
        public LimitCheckResult Check(LimitCheckContext context,List<RateLimitRule> rules)
        {
            if (string.IsNullOrWhiteSpace(context.Target))
            {
                throw new NotSupportedException("Null target is not supported");
            }
            var t = context.Target;
            foreach (var item in rules)
            {
                if (item is FixedWindowRule rule)
                {
                    t = $"{rule.Id}:{t}";
                    t = string.Intern(t);
                    var res = Passed(t, rule);
                    if (!res.passed)
                    {
                        return new LimitCheckResult
                        {
                            Passed = false,
                            Count = res.counter.Value,
                            StatWindow=res.counter.StatWindow,
                            PeriodTime=rule.WindowTime,
                            TotalCount = rule.LimitNumber
                        };
                    }
                }
            }

            return new LimitCheckResult
            {
                Passed = true
            };
        }

        /// <inheritdoc />
        public IEnumerable<RateLimitRule> Parse(string[] itemRule)
        {
            foreach (var item in itemRule)
            {
                var r = item.Split(":");
                yield return new FixedWindowRule
                {
                    Id=item,
                    WindowTime = TimeSpan.FromSeconds(int.Parse(r[0])),
                    LimitNumber = uint.Parse(r[1])
                };
            }
        }


        /// <summary>
        /// check single rule for target
        /// </summary>
        /// <param name="target"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected (bool passed,FixedWindowCounter counter) Passed(string target, FixedWindowRule rule)
        {
            lock (target)
            {
                var time = DateTimeOffset.UtcNow;
                var init = new FixedWindowCounter
                {
                    Value = 1,
                    StartTime = time,
                    StatWindow = TimeSpan.FromSeconds(0)
                };
                var counter = _cache.AddOrUpdate(target, s => init, (s, old) =>
                {
                    old.Value++;
                    old.StatWindow= time-old.StartTime;
                    if (old.StatWindow <= rule.WindowTime) return old;
                    old.Value = 1;
                    old.StatWindow=TimeSpan.FromSeconds(0);
                    old.StartTime = time;
                    return old;
                });

                if (counter.Value>rule.LimitNumber)
                {
                    return (false,counter);
                }
                return (true,counter);
            }
        }

    }
}

