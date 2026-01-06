using System.Linq.Expressions;
using RecipeManager.Api.Domain.Common.Abstractions;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Application.Database;

public interface IBaseRepository<TEntity, TKey>
           where TEntity : IEntity<TKey>
           where TKey : IEntityId
{
    Task Add(TEntity entity, CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filters = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? order = null, PagedParameters? paging = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    Task Update(TEntity entity, CancellationToken cancellationToken = default);

    Task Delete(TEntity entity, CancellationToken cancellationToken = default);
}
