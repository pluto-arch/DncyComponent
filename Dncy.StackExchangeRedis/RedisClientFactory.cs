#if NETCOREAPP

using Dotnetydd.StackExchangeRedis;
using System.Collections.Generic;
using System.Linq;

namespace Pluto.Redis
{
    public class RedisClientFactory
    {
        private readonly IEnumerable<IRedisClient> _clients;

        public RedisClientFactory(IEnumerable<IRedisClient> clients)
        {
            _clients = clients;
        }


        public IRedisClient this[string index] => _clients.FirstOrDefault(x => x.Name == index);
    }
}
#endif
