using System.Linq.Expressions;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Database.Repositories;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Persistance.Repositories;

public class CachedUnitRepository(IUnitRepository decorated, ICacheService cacheService) : IUnitRepository
{
    private const string KeyPrefix = "units";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public async Task Add(Unit entity, CancellationToken cancellationToken = default)
    {
        await decorated.Add(entity, cancellationToken);
        await RemoveCache(entity, cancellationToken);
    }

    public async Task<bool> AnyByNameAsync(string name, UnitId excludedId = null, CancellationToken cancellationToken = default)
    {
        return await decorated.AnyByNameAsync(name, excludedId, cancellationToken);
    }

    public async Task<bool> AnyByShortNameAsync(string name, UnitId excludedId = null, CancellationToken cancellationToken = default)
    {
        return await decorated.AnyByShortNameAsync(name, excludedId, cancellationToken);
    }

    public async Task Delete(Unit entity, CancellationToken cancellationToken = default)
    {
        await decorated.Delete(entity, cancellationToken);
        await RemoveCache(entity, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Unit, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return await decorated.ExistsAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<Unit>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        string key = $"{KeyPrefix}-all";
        return await cacheService.GetOrCreateAsync(key, async ct => await decorated.GetAllAsync(ct), DefaultExpiry, cancellationToken)
            ?? [];
    }

    public async Task<IEnumerable<Unit>> GetAsync(Expression<Func<Unit, bool>>? filters = null, Func<IQueryable<Unit>, IOrderedQueryable<Unit>>? order = null, PagedParameters? paging = null, CancellationToken cancellationToken = default)
    {
        return await decorated.GetAsync(filters, order, paging, cancellationToken);
    }

    public async Task<Unit?> GetByIdAsync(UnitId id, CancellationToken cancellationToken = default)
    {
        string key = $"{KeyPrefix}-{id.Value}";
        return await cacheService.GetOrCreateAsync(key, async ct => await decorated.GetByIdAsync(id, ct), DefaultExpiry, cancellationToken);
    }

    public async Task<IEnumerable<Unit>> GetPrimaryUnitsAsync(CancellationToken cancellationToken = default)
    {
        string key = $"{KeyPrefix}-primary";
        return await cacheService.GetOrCreateAsync(key, async ct => await decorated.GetPrimaryUnitsAsync(ct), DefaultExpiry, cancellationToken)
            ?? [];
    }

    public async Task Update(Unit entity, CancellationToken cancellationToken = default)
    {
        await decorated.Update(entity, cancellationToken);
        await RemoveCache(entity, cancellationToken);
    }

    private async Task RemoveCache(Unit unit, CancellationToken cancellationToken)
    {
        await cacheService.RemoveValue($"{KeyPrefix}-all", cancellationToken);
        await cacheService.RemoveValue($"{KeyPrefix}-primary", cancellationToken);
        if (unit.Id is not null)
        {
            await cacheService.RemoveValue($"{KeyPrefix}-{unit.Id.Value}", cancellationToken);
        }
    }
}
