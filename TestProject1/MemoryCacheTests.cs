using System.Threading.Tasks;
using AutoFixture;
using ClassLibrary1;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;

namespace TestProject1
{
    public class MemoryCacheTests
    {
        private Fixture _fixture;
        private MemoryCache _memoryCache;
        private MemoryCacheOptions _memoryCacheOptions;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _memoryCacheOptions = new MemoryCacheOptions();
            _memoryCache = new MemoryCache(_memoryCacheOptions);
        }

        [Test]
        public void GetOrCreate_InvokedTwiceByOneThread_InvokesFactoryOnce()
        {
            var key = _fixture.Create<string>();
            var value = _fixture.Create<string>();

            var fooMock = new Mock<IFoo>();
            fooMock.Setup(x => x.Baz()).Returns(value);
            var foo = fooMock.Object;

            _memoryCache.GetOrCreate(key, entry => foo.Baz());
            _memoryCache.GetOrCreate(key, entry => foo.Baz());

            fooMock.Verify(x => x.Baz(), Times.Once);
        }

        [Test]
        [Ignore("This proves that \"our\" usage of memory cache is not thread safe!")]
        public void GetOrCreate_InvokedByTwoThreads_InvokesFactoryOnce()
        {
            var key = _fixture.Create<string>();
            var value = _fixture.Create<string>();

            var fooMock = new Mock<IFoo>();
            fooMock.Setup(x => x.Baz()).Returns(value);
            var foo = fooMock.Object;

            Task.WaitAll(
                Task.Run(() => _memoryCache.GetOrCreate(key, entry => foo.Baz())),
                Task.Run(() => _memoryCache.GetOrCreate(key, entry => foo.Baz()))
            );

            fooMock.Verify(x => x.Baz(), Times.Once);
        }

        [Test]
        public void SafeGetOrCreate_InvokedByTwoThreads_InvokesFactoryOnce()
        {
            var key = _fixture.Create<string>();
            var value = _fixture.Create<string>();

            var fooMock = new Mock<IFoo>();
            fooMock.Setup(x => x.Baz()).Returns(value);
            var foo = fooMock.Object;

            Task.WaitAll(
                Task.Run(() => _memoryCache.SafeGetOrCreate(key, entry => foo.Baz())),
                Task.Run(() => _memoryCache.SafeGetOrCreate(key, entry => foo.Baz()))
            );

            fooMock.Verify(x => x.Baz(), Times.Once);
        }

        [TearDown]
        public void TearDown()
        {
            _memoryCache.Dispose();
        }
    }
}