using Microsoft.Extensions.Caching.Distributed;
using Supertext.Base.Common;

namespace Supertext.Base.Caching.Redis;

internal class RedisCache : IDecentralizedCache
{
    private readonly IDistributedCache _distributedCache;

    public RedisCache(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public byte[] Get(string key)
    {
        return _distributedCache.Get(key);
    }

    public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
    {
        return await _distributedCache.GetAsync(key, token);
    }

    public Option<string> GetString(string key, CancellationToken token = default)
    {
        var value = _distributedCache.GetString(key);
        return String.IsNullOrWhiteSpace(value) ? Option<string>.None() : Option<string>.Some(value);
    }

    public async Task<Option<string>> GetStringAsync(string key, CancellationToken token = default)
    {
        var value = await _distributedCache.GetStringAsync(key, token);
        return String.IsNullOrWhiteSpace(value) ? Option<string>.None() : Option<string>.Some(value);
    }

    public void Set(string key, byte[] value)
    {
        _distributedCache.Set(key, value);
    }

    public void Set(string key, byte[] value, TimeSpan timeToLive)
    {
        var options = CreateDistributedCacheEntryOptions(timeToLive);
        _distributedCache.Set(key, value, options);
    }

    public async Task SetAsync(string key, byte[] value, CancellationToken token = default)
    {
        await _distributedCache.SetAsync(key, value, token);
    }

    public async Task SetAsync(string key, byte[] value, TimeSpan timeToLive, CancellationToken token = default)
    {
        var options = CreateDistributedCacheEntryOptions(timeToLive);
        await _distributedCache.SetAsync(key, value, options, token);
    }

    public void SetString(string key, string value)
    {
        _distributedCache.SetString(key, value);
    }

    public void SetString(string key, string value, TimeSpan timeToLive)
    {
        var options = CreateDistributedCacheEntryOptions(timeToLive);
        _distributedCache.SetString(key, value, options);
    }

    public async Task SetStringAsync(string key, string value, CancellationToken token = default)
    {
        await _distributedCache.SetStringAsync(key, value, token);
    }

    public async Task SetStringAsync(string key, string value, TimeSpan timeToLive, CancellationToken token = default)
    {
        var options = CreateDistributedCacheEntryOptions(timeToLive);
        await _distributedCache.SetStringAsync(key, value, options, token);
    }

    public void Refresh(string key)
    {
        _distributedCache.Refresh(key);
    }

    public async Task RefreshAsync(string key, CancellationToken token = default)
    {
        await _distributedCache.RefreshAsync(key, token);
    }

    public void Remove(string key)
    {
        _distributedCache.Remove(key);
    }

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        await _distributedCache.RemoveAsync(key, token);
    }

    private static DistributedCacheEntryOptions CreateDistributedCacheEntryOptions(TimeSpan timeToLive)
    {
        return new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeToLive };
    }
}