using System;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Supertext.Base.Common;
using Supertext.Base.Threading;

[assembly: InternalsVisibleTo("Supertext.Base.NetFramework.Caching")]

namespace Supertext.Base.Caching.Caching
{
    internal class MemoryCache<T> : IMemoryCache<T> where T : class
    {
        private readonly object _clearingLock;
        private readonly AsyncDuplicateLock _syncLock = new();
        private readonly ICacheSettings _settings;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly MemoryCache _memoryCache;

        public MemoryCache(string cacheName, ICacheSettings settings, IDateTimeProvider dateTimeProvider)
        {
            _clearingLock = new object();
            _settings = settings;
            _dateTimeProvider = dateTimeProvider;
            Validate.NotEmpty(cacheName, nameof(cacheName));

            _memoryCache = new MemoryCache(cacheName);
        }

        public void Add(string key, T item)
        {
            Validate.NotEmpty(key, nameof(key));
            Validate.NotNull(item, nameof(item));

            using (_syncLock.Lock(_clearingLock))
            using (_syncLock.Lock(key))
            {
                _memoryCache.Set(key, item, _dateTimeProvider.UtcNow.AddSeconds(_settings.LifeTimeInSeconds));
            }
        }

        public void Clear()
        {
            using (_syncLock.Lock(_clearingLock))
            {
                foreach (var element in _memoryCache)
                {
                    _memoryCache.Remove(element.Key);
                }
            }
        }

        public Option<T> Get(string key)
        {
            Validate.NotEmpty(key, nameof(key));

            using (_syncLock.Lock(_clearingLock))
            using (_syncLock.Lock(key))
            {
                return _memoryCache.Get(key) is T item
                           ? Option<T>.Some(item)
                           : Option<T>.None();
            }
        }

        public T GetOrCreateAndGet(string key, Func<string, T> factoryMethod)
        {
            Validate.NotEmpty(key, nameof(key));
            Validate.NotNull(factoryMethod, nameof(factoryMethod));

            using (_syncLock.Lock(_clearingLock))
            using (_syncLock.Lock(key))
            {
                if (_memoryCache.Get(key) is not T result)
                {
                    result = factoryMethod(key);
                    _memoryCache.Set(key, result, _dateTimeProvider.UtcNow.AddSeconds(_settings.LifeTimeInSeconds));
                }

                return result;
            }
        }

        public async Task<T> GetOrCreateAndGetAsync(string key, Func<string, Task<T>> factoryMethod)
        {
            Validate.NotEmpty(key, nameof(key));
            Validate.NotNull(factoryMethod, nameof(factoryMethod));

            using (await _syncLock.LockAsync(_clearingLock).ConfigureAwait(false))
            using (await _syncLock.LockAsync(key).ConfigureAwait(false))
            {
                if (_memoryCache.Get(key) is not T result)
                {
                    result = await factoryMethod(key).ConfigureAwait(false);
                    _memoryCache.Set(key, result, _dateTimeProvider.UtcNow.AddSeconds(_settings.LifeTimeInSeconds));
                }

                return result;
            }
        }

        public void Remove(string key)
        {
            Validate.NotEmpty(key, nameof(key));

            using (_syncLock.Lock(_clearingLock))
            using (_syncLock.Lock(key))
            {
                _memoryCache.Remove(key);
            }
        }
    }
}