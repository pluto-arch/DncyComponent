namespace Dncy.RateLimit.Core
{
    public class LimitCheckContext
    {
        public LimitCheckContext(string target)
        {
            Target=target;
        }
        public string Target { get; set; }
    }
}

