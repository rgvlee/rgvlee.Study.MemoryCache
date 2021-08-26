using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using BenchmarkDotNet.Attributes;
using ClassLibrary1;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;

namespace ConsoleApp1
{
    [MemoryDiagnoser]
    public class SingleThreadBenchmarks
    {
        private List<KeyValuePair<string, string>> _cacheEntries = new List<KeyValuePair<string, string>>();

        private CachingService _cachingService;
        private Fixture _fixture;
        private MemoryCache _memoryCache;
        private MemoryCacheOptions _memoryCacheOptions;

        [Params(500, 5000, 50000)]
        //[Params(500)]
        public int NumberOfCacheEntries;

        [GlobalSetup]
        public void Setup()
        {
            _fixture = new Fixture();
            _cacheEntries = _fixture.CreateMany<KeyValuePair<string, string>>(NumberOfCacheEntries).ToList();
            _memoryCacheOptions = new MemoryCacheOptions();
            _memoryCache = new MemoryCache(_memoryCacheOptions);

            _cachingService = new CachingService();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _memoryCache.Dispose();
            _cachingService.CacheProvider.Dispose();
        }

        [Benchmark]
        public List<string> GetOrCreate()
        {
            var cacheValues = new List<string>(NumberOfCacheEntries);
            foreach (var (key, value) in _cacheEntries)
            {
                cacheValues.Add(_memoryCache.GetOrCreate(key, entry => value));
            }

            return cacheValues;
        }

        [Benchmark]
        public List<string> SafeGetOrCreate()
        {
            var cacheValues = new List<string>(NumberOfCacheEntries);
            foreach (var (key, value) in _cacheEntries)
            {
                cacheValues.Add(_memoryCache.SafeGetOrCreate(key, entry => value));
            }

            return cacheValues;
        }

        [Benchmark]
        public List<string> GetOrAdd()
        {
            var cacheValues = new List<string>(NumberOfCacheEntries);
            foreach (var (key, value) in _cacheEntries)
            {
                cacheValues.Add(_cachingService.GetOrAdd(key, entry => value));
            }

            return cacheValues;
        }
    }
}