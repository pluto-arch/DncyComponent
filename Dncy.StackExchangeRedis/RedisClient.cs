
using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Dotnetydd.StackExchangeRedis
{


    public interface IRedisClient
    {
        IDatabase Db { get; }

        string Name { get; }
    }



#if !NET461
    public class RedisClient : IRedisClient, IDisposable, IAsyncDisposable
#else
    public class RedisClient : IRedisClient, IDisposable
#endif
    {
        private Lazy<ConnectionMultiplexer> connectionLazy = new Lazy<ConnectionMultiplexer>();

        private readonly ConfigurationOptions _options;
        private bool disposedValue;


        /// <summary>
        /// 初始化 <see cref="RedisClient"/> 类的新实例。
        /// </summary>
        public RedisClient(ConfigurationOptions options)
        {
            _options = options;
            _ = options ?? throw new ArgumentNullException("options can not be null");
            InitConnection();
        }



        private readonly string _configString;
        private readonly Action<ConfigurationOptions> _confAction;
        /// <summary>
        /// 初始化 <see cref="RedisClient"/> 类的新实例。
        /// </summary>
        public RedisClient(string configuration,Action<ConfigurationOptions> options)
        {
            _confAction = options;
            _configString = configuration;
            InitConnection();
        }


        void InitConnection()
        {
            connectionLazy = new Lazy<ConnectionMultiplexer>(GetConnection);
        }
        ConnectionMultiplexer GetConnection()
        {
            if (!string.IsNullOrEmpty(_configString))
            {
                return ConnectionMultiplexer.Connect(_configString,_confAction);
            }
            return ConnectionMultiplexer.Connect(_options);
        }


        public IDatabase Db => GetDatabase();

        public ISubscriber Sub => connectionLazy.Value.GetSubscriber();

        public string Name => _options.ClientName;


        public IDatabase this[int index]
        {
            get
            {
                if (index < 0 || index > 16)
                {
                    throw new IndexOutOfRangeException(nameof(index));
                }

                return GetDatabase(index);
            }
        }


        /// <summary>
        /// 获取redis db
        /// </summary>
        /// <returns></returns>
        private IDatabase GetDatabase()
        {
            return connectionLazy.Value.GetDatabase(_options.DefaultDatabase ?? 0);
        }

        /// <summary>
        /// 获取redis db
        /// </summary>
        /// <returns></returns>
        private IDatabase GetDatabase(int dbNumber)
        {
            return connectionLazy.Value.GetDatabase(dbNumber);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    connectionLazy.Value?.Close();
                    connectionLazy.Value?.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        #region dispose


        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~PlutoRedisClient()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

#if NETCOREAPP
        public ValueTask DisposeAsync()
        {
            Dispose(true);
            return ValueTask.CompletedTask;
        }
#endif


        #endregion

    }
}
