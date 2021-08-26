using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace ClassLibrary1
{
    public static class MemoryCacheExtensions
    {
        private static readonly ConcurrentDictionary<object, object> CacheKeyLocks = new ConcurrentDictionary<object, object>();

        public static TItem SafeGetOrCreate<TItem>(this IMemoryCache cache, object key, Func<ICacheEntry, TItem> factory)
        {
            lock (CacheKeyLocks.GetOrAdd(key, new object()))
            {
                return cache.GetOrCreate(key, factory);
            }
        }
    }
}