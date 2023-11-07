﻿using Microsoft.Extensions.Caching.Distributed;

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

    public void Set(string key, byte[] value)
    {
        _distributedCache.Set(key, value);
    }

    public void Set(string key, byte[] value, TimeSpan absoluteExpirationRelativeToNow)
    {
        var options = CreateDistributedCacheEntryOptions(absoluteExpirationRelativeToNow);
        _distributedCache.Set(key, value, options);
    }

    public async Task SetAsync(string key, byte[] value, CancellationToken token = default)
    {
        await _distributedCache.SetAsync(key, value, token);
    }

    public async Task SetAsync(string key, byte[] value, TimeSpan absoluteExpirationRelativeToNow, CancellationToken token = default)
    {
        var options = CreateDistributedCacheEntryOptions(absoluteExpirationRelativeToNow);
        await _distributedCache.SetAsync(key, value, options, token);
    }

    public void SetString(string key, string value)
    {
        _distributedCache.SetString(key, value);
    }

    public void SetString(string key, string value, TimeSpan absoluteExpirationRelativeToNow)
    {
        var options = CreateDistributedCacheEntryOptions(absoluteExpirationRelativeToNow);
        _distributedCache.SetString(key, value, options);
    }

    public async Task SetStringAsync(string key, string value, CancellationToken token = default)
    {
        await _distributedCache.SetStringAsync(key, value, token);
    }

    public async Task SetStringAsync(string key, string value, TimeSpan absoluteExpirationRelativeToNow, CancellationToken token = default)
    {
        var options = CreateDistributedCacheEntryOptions(absoluteExpirationRelativeToNow);
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

    private static DistributedCacheEntryOptions CreateDistributedCacheEntryOptions(TimeSpan absoluteExpirationRelativeToNow)
    {
        return new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow };
    }
}