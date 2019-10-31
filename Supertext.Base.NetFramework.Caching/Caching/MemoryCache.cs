using System;
using System.Runtime.Caching;
using Supertext.Base.Caching;
using Supertext.Base.Common;

namespace Supertext.Base.NetFramework.Caching.Caching
{
    internal class MemoryCache<T> : IMemoryCache<T> where T : class
    {
        private readonly ICacheSettings _settings;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly object _lock = new object();
        private readonly MemoryCache _memoryCache;

        public MemoryCache(string cacheName, ICacheSettings settings, IDateTimeProvider dateTimeProvider)
        {
            _settings = settings;
            _dateTimeProvider = dateTimeProvider;
            Validate.NotEmpty(cacheName, nameof(cacheName));

            _memoryCache = new MemoryCache(cacheName);
        }

        public void Add(string key, T item)
        {
            Validate.NotEmpty(key, nameof(key));
            Validate.NotNull(item, nameof(item));

            lock (_lock)
            {
                _memoryCache.Set(key, item, _dateTimeProvider.UtcNow.AddSeconds(_settings.LifeTimeInSeconds));
            }
        }

        public Option<T> Get(string key)
        {
            Validate.NotEmpty(key, nameof(key));

            lock (_lock)
            {
                return _memoryCache.Get(key) is T item ? Option<T>.Some(item) : Option<T>.None();
            }
        }

        public T GetOrCreateAndGet(string key, Func<string, T> factoryMethod)
        {
            Validate.NotEmpty(key, nameof(key));
            Validate.NotNull(factoryMethod, nameof(factoryMethod));

            lock (_lock)
            {
                if (!(_memoryCache.Get(key) is T result))
                {
                    result = factoryMethod(key);
                    Add(key, result);
                }

                return result;
            }
        }

        public void Remove(string key)
        {
            Validate.NotEmpty(key, nameof(key));

            lock (_lock)
            {
                _memoryCache.Remove(key);
            }
        }
    }
}