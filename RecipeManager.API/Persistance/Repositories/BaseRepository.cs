using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Domain.Common.Abstractions;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Persistance.Repositories;

public abstract class BaseRepository<TEntity, TKey>(AppDbContext dbContext, ICurrentUser currentUser) : IBaseRepository<TEntity, TKey>
                where TEntity : class, IEntity<TKey>
                where TKey : IEntityId
{
    public async Task Add(TEntity entity, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public Task Delete(TEntity entity, CancellationToken cancellationToken = default)
    {
        dbContext.Set<TEntity>().Remove(entity);

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Func<TEntity>? predicate = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TEntity>> GetAsync(Func<TEntity>? filters = null, Func<TEntity>? order = null, PagedParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<TEntity>().FindAsync([id], cancellationToken);
    }

    public Task Update(TEntity entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
