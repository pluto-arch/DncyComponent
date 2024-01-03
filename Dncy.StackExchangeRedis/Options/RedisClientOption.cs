using System;
using System.Collections.Generic;
using System.Net;
using StackExchange.Redis;

namespace Dotnetydd.StackExchangeRedis.Options
{
    public class RedisClientOption
    {
        public string InstanceName { get; set; }

        public byte DefaultDataBase { get; set; }

        public string Password { get; set; }

        public string MasterName { get; set; }

        public int KeepAlive { get; set; } = 180;

        public int SyncTimeout { get; set; } = 3000;

        public int ConnectTimeout { get; set; } = 3000;

        public bool AllowAdmin { get; set; }

        public Dictionary<string, int> RedisAddress { get; set; }

        public CommandMap CommandMap { get; set; }

        public Version Version { get; set; }
    }
}