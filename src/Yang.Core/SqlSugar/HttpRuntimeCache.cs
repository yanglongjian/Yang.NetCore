using SqlSugar;
using System;
using System.Collections.Generic;

namespace Yang.Core
{
    /// <summary>
    /// SqlSugar二级缓存
    /// </summary>
    public class HttpRuntimeCache : ICacheService
    {
        public void Add<V>(string key, V value)
        {
            RedisHelper.Set(key, value);
        }

        public void Add<V>(string key, V value, int cacheDurationInSeconds)
        {
            RedisHelper.Set(key, value, cacheDurationInSeconds);
        }

        public bool ContainsKey<V>(string key)
        {
            return RedisHelper.Exists(key);
        }

        public V Get<V>(string key)
        {
            return RedisHelper.Get<V>(key);
        }

        public IEnumerable<string> GetAllKey<V>()
        {
            return RedisHelper.Keys("*");
        }

        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            if (RedisHelper.Exists(cacheKey))
            {
                return RedisHelper.Get<V>(cacheKey);
            }
            else
            {
                var result = create();
                RedisHelper.Set(cacheKey, result, cacheDurationInSeconds);
                return result;
            }
        }

        public void Remove<V>(string key)
        {
            RedisHelper.Del(key);
        }
    }
}



