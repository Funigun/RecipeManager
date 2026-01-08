using Microsoft.Extensions.Caching.Hybrid;
using RecipeManager.Api.Application.Abstractions;

namespace RecipeManager.Api.Persistance.Cache;

public sealed class HybridCacheService(HybridCache hybridCache) : ICacheService
{
    public async Task SetValue<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
         where T : class
    {
        HybridCacheEntryOptions? options = expiry.HasValue ? new HybridCacheEntryOptions { Expiration = expiry } : null;
        await hybridCache.SetAsync(key, value, options, cancellationToken: cancellationToken);
    }

    public async Task<T?> GetValue<T>(string key, CancellationToken cancellationToken = default)
             where T : class
    {
        return await hybridCache.GetOrCreateAsync<T?>(key, cancellationToken => ValueTask.FromResult<T?>(null), cancellationToken: cancellationToken);
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<CancellationToken, ValueTask<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        HybridCacheEntryOptions? options = expiry.HasValue ? new HybridCacheEntryOptions { Expiration = expiry } : null;
        return await hybridCache.GetOrCreateAsync(key, factory, options, tags: null, cancellationToken: cancellationToken);
    }

    public async Task RemoveValue(string key, CancellationToken cancellationToken = default)
    {
        await hybridCache.RemoveAsync(key, cancellationToken);
    }
}
