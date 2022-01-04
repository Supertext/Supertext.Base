using System;
using System.Threading.Tasks;
using Supertext.Base.Common;

namespace Supertext.Base.Caching
{
    public interface IMemoryCache<T> where T : class
    {
        void Add(string key, T item);
        Option<T> Get(string key);
        T GetOrCreateAndGet(string key, Func<string, T> factoryMethod);
        Task<T> GetOrCreateAndGetAsync(string key, Func<string, Task<T>> factoryMethod);
        void Remove(string key);
    }
}