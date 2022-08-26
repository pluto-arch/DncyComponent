namespace Dncy.RateLimit.Core
{
    public abstract class RateLimitRule
    {
        protected RateLimitRule()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public abstract uint GetLimitThreshold();

        public string Id { get; set; }
    }
}

