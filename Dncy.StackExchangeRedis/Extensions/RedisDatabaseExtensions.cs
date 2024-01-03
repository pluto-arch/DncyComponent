using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Dotnetydd.StackExchangeRedis.Extensions
{
    public static class RedisStringOperates
    {
        #region 同步
        /// <summary>
        /// 添加一个字符串对象。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        public static bool Set(this IDatabase db, string key, string value, TimeSpan? expiry = null) => db.StringSet(key, value, expiry);

        /// <summary>
        /// 添加一个字符串对象。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        public static bool Set(this IDatabase db, string key, string value, int seconds) => db.StringSet(key, value, TimeSpan.FromSeconds(seconds));


        /// <summary>
        /// 添加一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="serializeFunc"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public static bool Set<T>(this IDatabase db, string key, T data, Func<T, string> serializeFunc, TimeSpan? expiry = null) => db.StringSet(key, serializeFunc.Invoke(data), expiry);


        /// <summary>
        /// 添加一个对象。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="serializeFunc"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static bool Set<T>(this IDatabase db, string key, T data, Func<T, string> serializeFunc, int seconds) => db.StringSet(key, serializeFunc.Invoke(data), TimeSpan.FromSeconds(seconds));

        /// <summary>
        /// 获取一个对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static T Get<T>(this IDatabase db, string key, Func<string, T> callback) => callback(db.StringGet(key));

        /// <summary>
        /// 获取一个字符串对象。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">值。</param>
        /// <returns>返回对象的值。</returns>
        public static string Get(this IDatabase db, string key) => db.StringGet(key);

        /// <summary>
        /// 删除一个对象。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">键。</param>
        /// <returns>返回是否执行成功。</returns>
        public static bool Delete(this IDatabase db, string key) => db.KeyDelete(key);

        /// <summary>
        /// 返回键是否存在。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">键。</param>
        /// <returns>返回键是否存在。</returns>
        public static bool Exists(this IDatabase db, string key) => db.KeyExists(key);

        /// <summary>
        /// 设置一个键的过期时间。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">键。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        public static bool SetExpire(this IDatabase db, string key, TimeSpan? expiry) => db.KeyExpire(key, expiry);

        /// <summary>
        /// 设置一个键的过期时间。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">键。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        public static bool SetExpire(this IDatabase db, string key, int seconds) => db.KeyExpire(key, TimeSpan.FromSeconds(seconds));


        #endregion

        #region 异步

        /// <summary>
        /// 异步添加一个字符串对象。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public static async Task<bool> SetAsync(this IDatabase db, string key, string value, TimeSpan? expiry = null) => await db.StringSetAsync(key, value, expiry);


        /// <summary>
        /// 异步添加一个字符串对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        public static async Task<bool> SetAsync(this IDatabase db, string key, string value, int seconds)
            => await db.StringSetAsync(key, value, TimeSpan.FromSeconds(seconds));


        /// <summary>
        /// 添加一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="serializeFunc"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public static async Task<bool> SetAsync<T>(this IDatabase db, string key, T data, Func<T, string> serializeFunc, TimeSpan? expiry = null) => await db.StringSetAsync(key, serializeFunc.Invoke(data), expiry);


        /// <summary>
        /// 添加一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="serializeFunc"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static async Task<bool> SetAsync<T>(this IDatabase db, string key, T data, Func<T, string> serializeFunc, int second) => await db.StringSetAsync(key, serializeFunc.Invoke(data), TimeSpan.FromSeconds(second));


        /// <summary>
        /// 异步获取一个对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(this IDatabase db, string key, Func<string, T> callback) => callback.Invoke(await db.StringGetAsync(key));

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">值。</param>
        /// <returns>返回对象的值。</returns>
        public static async Task<string> GetAsync(this IDatabase db, string key) => await db.StringGetAsync(key);

        /// <summary>
        /// 删除一个对象。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">键。</param>
        /// <returns>返回是否执行成功。</returns>
        public static async Task<bool> DeleteAsync(this IDatabase db, string key) => await db.KeyDeleteAsync(key);

        /// <summary>
        /// 设置一个键的过期时间。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">键。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        public static async Task<bool> SetExpireAsync(this IDatabase db, string key, int seconds) => await db.KeyExpireAsync(key, TimeSpan.FromSeconds(seconds));

        /// <summary>
        /// 设置一个键的过期时间。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">键。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        public static async Task<bool> SetExpireAsync(this IDatabase db, string key, TimeSpan? expiry) => await db.KeyExpireAsync(key, expiry);

        #endregion
    }

    public static class RedisDistributedLockOperate
    {
        /// <summary>
        /// 获取锁。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">锁名称。</param>
        /// <param name="token"></param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>是否已锁。</returns>
        public static bool Lock(this IDatabase db, string key, string token, int seconds) => db.LockTake(key, token, TimeSpan.FromSeconds(seconds));

        /// <summary>
        /// 释放锁。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="token"></param>
        /// <param name="key">锁名称。</param>
        /// <returns>是否成功。</returns>
        public static bool UnLock(this IDatabase db, string token, string key) => db.LockRelease(key, token);

        /// <summary>
        /// 异步获取锁。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key">锁名称。</param>
        /// <param name="token"></param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>是否成功。</returns>
        public static async Task<bool> LockAsync(this IDatabase db, string key, string token, int seconds) => await db.LockTakeAsync(key, token, TimeSpan.FromSeconds(seconds));

        /// <summary>
        /// 异步释放锁。
        /// </summary>
        /// <param name="token"></param>
        /// <param name="key">锁名称。</param>
        /// <param name="db"></param>
        /// <returns>是否成功。</returns>
        public static async Task<bool> UnLockAsync(this IDatabase db, string token, string key) => await db.LockReleaseAsync(key, token);
    }

    public static class RedisListOperates
    {

    }

    public static class RedisHashOperates
    {

    }

    public static class RedisPubSubOperates
    {
        public static long Publish(this ISubscriber subscriber, string channel, Func<string> serializaFun)
        {
            return subscriber.Publish(channel, serializaFun.Invoke());
        }

        public static void Subscribe(this ISubscriber subscriber, string subChannel, Action<RedisChannel, string> callback)
        {
            subscriber.Subscribe(subChannel, (channel, message) =>
            {
                callback(channel, message);
            });
        }


        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="channel"></param>
        public static void Unsubscribe(this ISubscriber subscriber, string channel)
        {
            subscriber.Unsubscribe(channel);
        }
        /// <summary>
        /// 取消所有订阅
        /// </summary>
        /// <param name="subscriber"></param>
        public static void UnsubscribeAll(this ISubscriber subscriber)
        {
            subscriber.UnsubscribeAll();
        }
    }
}