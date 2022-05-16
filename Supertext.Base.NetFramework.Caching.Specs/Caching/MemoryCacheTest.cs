using System;
using System.Threading.Tasks;
using Autofac;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Caching;
using Supertext.Base.Common;
using Supertext.Base.NetFramework.Caching.Caching;

namespace Supertext.Base.NetFramework.Caching.Specs.Caching
{
    [TestClass]
    public class MemoryCacheTest
    {
        private IContainer _container;
        private IDateTimeProvider _dateTimeProvider;
        private CacheSettings _cacheSettings;

        [TestInitialize]
        public void TestInitialize()
        {
            _container = SetUpContainer();
            A.CallTo(() => _dateTimeProvider.UtcNow).Returns(DateTime.UtcNow);
        }

        [TestMethod]
        public void ResolveMemoryCache_SameItemIsProvided_ReturnsSameMemoryCache()
        {
            //Arrange
            var testee1 = _container.Resolve<IMemoryCache<CacheItem1>>();
            var testee2 = _container.Resolve<IMemoryCache<CacheItem1>>();

            //Assert
            testee1.Should().Be(testee2);
        }

        [TestMethod]
        public void Get_CacheItemNotExist_CacheReturnsNone()
        {
            //Arrange
            var testee = _container.Resolve<IMemoryCache<CacheItem1>>();

            //Act
            var result = testee.Get("key1");

            //Assert
            result.IsNone.Should().BeTrue();
        }

        [TestMethod]
        public void Get_CacheItemIsAdded_CacheReturnsItem()
        {
            //Arrange
            var testee = _container.Resolve<IMemoryCache<CacheItem1>>();
            _cacheSettings.LifeTimeInSeconds = 10;
            var cacheItem = new CacheItem1();
            testee.Add("key1", cacheItem);

            //Act
            var result = testee.Get("key1");

            //Assert
            result.IsSome.Should().BeTrue();
            result.Value.Should().Be(cacheItem);
        }

        [TestMethod]
        public void Get_CacheItemIsExpired_CacheReturnsNone()
        {
            //Arrange
            var testee = _container.Resolve<IMemoryCache<CacheItem1>>();
            _cacheSettings.LifeTimeInSeconds = 0;
            testee.Add("key1", new CacheItem1());

            //Act
            var result = testee.Get("key1");

            //Assert
            result.IsNone.Should().BeTrue();
        }

        [TestMethod]
        public void Get_TheSameCacheItemIsAddedTwice_CacheReturnsItem()
        {
            //Arrange
            var testee = _container.Resolve<IMemoryCache<CacheItem1>>();
            _cacheSettings.LifeTimeInSeconds = 10;
            var cacheItem = new CacheItem1();
            testee.Add("key1", cacheItem);
            testee.Add("key1", cacheItem);

            //Act
            var result = testee.Get("key1");

            //Assert
            result.IsSome.Should().BeTrue();
            result.Value.Should().Be(cacheItem);
        }

        [TestMethod]
        public void Get_TheSameCacheItemIsAddedTwiceForDifferentKey_CacheReturnsItem()
        {
            //Arrange
            var testee = _container.Resolve<IMemoryCache<CacheItem1>>();
            _cacheSettings.LifeTimeInSeconds = 10;
            var cacheItem = new CacheItem1();
            testee.Add("key1", cacheItem);
            testee.Add("key2", cacheItem);

            //Act
            var result1 = testee.Get("key1");
            var result2 = testee.Get("key2");

            //Assert
            result1.IsSome.Should().BeTrue();
            result1.Value.Should().Be(cacheItem);
            result2.IsSome.Should().BeTrue();
            result2.Value.Should().Be(cacheItem);
            result1.Should().Be(result2);
        }

        [TestMethod]
        public void Get_CacheItemIsAddedInDifferentInstancesForTheSameKey_CacheReturnsLatestAddedItem()
        {
            //Arrange
            _cacheSettings.LifeTimeInSeconds = 10;
            var testee1 = _container.Resolve<IMemoryCache<CacheItem1>>();
            var cacheItem1 = new CacheItem1();
            testee1.Add("key1", cacheItem1);

            var cacheItem2 = new CacheItem1();
            testee1.Add("key1", cacheItem2);

            //Act
            var resultCacheItem1 = testee1.Get("key1");

            //Assert
            resultCacheItem1.IsSome.Should().BeTrue();

            resultCacheItem1.Value.Should().NotBe(cacheItem1);
            resultCacheItem1.Value.Should().Be(cacheItem2);
        }

        [TestMethod]
        public void Remove_CacheItemExist_CacheReturnsNone()
        {
            //Arrange
            var testee = _container.Resolve<IMemoryCache<CacheItem1>>();
            testee.Add("key1", new CacheItem1());

            //Act
            testee.Remove("key1");
            var result = testee.Get("key1");

            //Assert
            result.IsNone.Should().BeTrue();
        }

        [TestMethod]
        public void GetOrCreateAndGet_CacheItemIsAdded_CacheReturnsItem()
        {
            //Arrange
            var testee = _container.Resolve<IMemoryCache<CacheItem1>>();
            _cacheSettings.LifeTimeInSeconds = 10;
            var cacheItem = new CacheItem1();

            //Act
            var result = testee.GetOrCreateAndGet("key1", s => cacheItem);

            //Assert
            result.Should().Be(cacheItem);
        }

        [TestMethod]
        public void GetOrCreateAndGet_CacheItemIsExpired_CacheReturnsItem()
        {
            //Arrange
            var testee = _container.Resolve<IMemoryCache<CacheItem1>>();
            _cacheSettings.LifeTimeInSeconds = 0;
            var cacheItem = new CacheItem1();
            testee.Add("key1", cacheItem);
            var isCreatedByFactoryMethod = false;

            //Act
            var result = testee.GetOrCreateAndGet("key1",
                                                  s =>
                                                  {
                                                      isCreatedByFactoryMethod = true;
                                                      return new CacheItem1();
                                                  });

            //Assert
            result.Should().NotBe(cacheItem);
            isCreatedByFactoryMethod.Should().BeTrue();
        }

        [TestMethod]
        public async Task GetOrCreateAndGetAsync_CacheItemIsAdded_CacheReturnsItem()
        {
            //Arrange
            var testee = _container.Resolve<IMemoryCache<CacheItem1>>();
            _cacheSettings.LifeTimeInSeconds = 10;
            var cacheItem = new CacheItem1();

            //Act
            var result = await testee.GetOrCreateAndGetAsync("key1", async key => await Task.FromResult(cacheItem));

            //Assert
            result.Should().Be(cacheItem);
        }

        [TestMethod]
        public async Task GetOrCreateAndGetAsync_CacheItemIsExpired_CacheReturnsItem()
        {
            //Arrange
            var testee = _container.Resolve<IMemoryCache<CacheItem1>>();
            _cacheSettings.LifeTimeInSeconds = 0;
            var cacheItem = new CacheItem1();
            testee.Add("key1", cacheItem);
            var isCreatedByFactoryMethod = false;

            //Act
            var result = await testee.GetOrCreateAndGetAsync("key1",
                                                             s =>
                                                             {
                                                                 isCreatedByFactoryMethod = true;
                                                                 return Task.FromResult(new CacheItem1());
                                                             });

            //Assert
            result.Should().NotBe(cacheItem);
            isCreatedByFactoryMethod.Should().BeTrue();
        }

        private IContainer SetUpContainer()
        {
            var containerBuilder = new ContainerBuilder();

            _cacheSettings = new CacheSettings();
            containerBuilder.RegisterInstance(_cacheSettings).As<CacheSettings>();
            _dateTimeProvider = A.Fake<IDateTimeProvider>();
            containerBuilder.RegisterInstance(_dateTimeProvider);

#pragma warning disable CS0618
            containerBuilder.RegisterCache<CacheItem1, CacheSettings>("cacheItem1");
            containerBuilder.RegisterCache<CacheItem2, CacheSettings>("cacheItem2");
#pragma warning restore CS0618

            return containerBuilder.Build();
        }

        internal class CacheItem1
        {
        }

        internal class CacheItem2
        {
        }

        internal class CacheSettings : ICacheSettings
        {
            public int LifeTimeInSeconds { get; set; }
        }
    }
}