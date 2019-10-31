using System;
using Supertext.Base.Common;

namespace Supertext.Base.Caching
{
    public interface IMemoryCache<T> where T : class
    {
        void Add(string key, T item);
        Option<T> Get(string key);
        T GetOrCreateAndGet(string key, Func<string, T> factoryMethod);
        void Remove(string key);
    }
}