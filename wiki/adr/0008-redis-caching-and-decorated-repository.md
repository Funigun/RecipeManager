# 8. Redis Caching and Decorated Repository Pattern

Date: 2024-05-23
Status: Accepted

## Context

Efficient data access and scalability are critical for RecipeManager, especially for frequently accessed resources such as measurement units. To reduce database load and improve response times, we need a robust caching strategy. At the same time, we want flexibility to swap cache providers without major code changes.

## Decision

We use Redis as our distributed cache, managed via Aspire orchestration, and implement the Decorator pattern for repository caching.

### Redis Setup and Registration
*   **Container Creation**: The Redis container is provisioned by Aspire in the application host (`RecipeManager.AppHost/Program.cs`). This ensures Redis is available for local development and deployment scenarios.
*   **Connection**: In `RecipeManager.API/Program.cs`, we connect to Redis using the HybridCache abstraction. This allows us to easily swap cache providers (e.g., in-memory, Redis, distributed) by changing configuration, not code.
*   **HybridCache**: The HybridCache library is used to wrap Redis and provide a unified caching API. This abstraction supports fallback and provider switching, improving maintainability and testability.

### Decorated Repository Pattern
*   **`CachedUnitRepository`**: Implements `IUnitRepository` and wraps a decorated repository instance. It intercepts calls to fetch units and applies caching logic for read operations (`GetAllAsync`, `GetByIdAsync`, `GetPrimaryUnitsAsync`).
*   **Cache Invalidation**: On write operations (`Add`, `Update`, `Delete`), the cache is invalidated for affected keys to ensure consistency.
*   **Fallback**: If a cache miss occurs, the repository fetches from the database and updates the cache.
*   **Expiry**: Cached entries use a default expiry (e.g., 1 hour) to balance freshness and performance.

### Example

**Aspire Host Redis Setup:**
```csharp
IResourceBuilder<RedisResource> redisCache = builder.AddRedis("Cache").WithRedisInsight();
```

**API Redis Registration:**
```csharp
builder.AddRedisClient("Cache");
// ...
builder.Services.AddStackExchangeRedisCache(opt => opt.ConnectionMultiplexerFactory = ...);
builder.Services.AddHybridCache(...);
builder.Services.AddSingleton<ICacheService, HybridCacheService>();
```

**Cached Repository Usage:**
```csharp
public class CachedUnitRepository(ICacheService cacheService) : IUnitRepository
{
    // ...
    public async Task<IEnumerable<Unit>> GetAllAsync(...) {
        return await cacheService.GetOrCreateAsync(key, async ct => await decorated.GetAllAsync(ct), expiry, cancellationToken);
    }
    // ...
}
```

## Consequences

### Positive
*   **Performance**: Reduces database load and improves API response times for frequently accessed data.
*   **Flexibility**: Cache provider can be swapped (e.g., Redis, in-memory) with minimal code changes due to HybridCache abstraction.
*   **Consistency**: Decorator ensures cache is invalidated on writes, maintaining data integrity.
*   **Scalability**: Redis supports distributed caching for multi-instance deployments.

### Negative
*   **Complexity**: Adds infrastructure and code complexity (container orchestration, cache invalidation logic).
*   **Stale Data Risk**: Cached data may be stale between invalidation and expiry, though mitigated by short expiry and targeted invalidation.
*   **Startup Overhead**: Redis container must be available and running for full cache functionality.
