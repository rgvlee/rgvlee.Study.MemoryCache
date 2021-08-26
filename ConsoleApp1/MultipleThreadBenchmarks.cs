using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using BenchmarkDotNet.Attributes;
using ClassLibrary1;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;

namespace ConsoleApp1
{
    [MemoryDiagnoser]
    public class MultipleThreadBenchmarks
    {
        private List<KeyValuePair<string, string>> _cacheEntries = new List<KeyValuePair<string, string>>();

        private CachingService _cachingService;
        private Fixture _fixture;
        private MemoryCache _memoryCache;
        private MemoryCacheOptions _memoryCacheOptions;

        [Params(500, 5000, 50000)]
        //[Params(100)]
        public int NumberOfCacheEntries;

        [Params(1, 2, 4, 8)]
        public int NumberOfThreads;

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
        public List<string> SafeGetOrCreate()
        {
            var cacheValues = new List<string>(NumberOfCacheEntries * NumberOfThreads);
            Parallel.ForEach(Enumerable.Range(1, NumberOfThreads), i =>
            {
                foreach (var (key, value) in _cacheEntries)
                {
                    cacheValues.Add(_memoryCache.SafeGetOrCreate(key, entry => value));
                }
            });

            return cacheValues;
        }

        [Benchmark]
        public List<string> GetOrAdd()
        {
            var cacheValues = new List<string>(NumberOfCacheEntries * NumberOfThreads);
            Parallel.ForEach(Enumerable.Range(1, NumberOfThreads), i =>
            {
                foreach (var (key, value) in _cacheEntries)
                {
                    cacheValues.Add(_cachingService.GetOrAdd(key, entry => value));
                }
            });

            return cacheValues;
        }
    }
}