using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Domain.Common.Abstractions;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Persistance.Repositories;

public abstract class BaseRepository<TEntity, TKey>(AppDbContext dbContext, ICurrentUser currentUser) : IBaseRepository<TEntity, TKey>
                where TEntity : class, IEntity<TKey>
                where TKey : class, IEntityId
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

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filters = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? order = null, PagedParameters? paging = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = dbContext.Set<TEntity>().AsNoTracking();

        if (filters is not null)
        {
            query = query.Where(filters);
        }

        if (order is not null)
        {
            query = order(query);
        }

        if (paging is not null)
        {
            query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return predicate is null
            ? await dbContext.Set<TEntity>().AnyAsync(cancellationToken)
            : await dbContext.Set<TEntity>().AnyAsync(predicate, cancellationToken);
    }

    public Task Update(TEntity entity, CancellationToken cancellationToken = default)
    {
        dbContext.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }
}
