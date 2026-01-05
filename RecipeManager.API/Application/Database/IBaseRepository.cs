using RecipeManager.Api.Domain.Common.Abstractions;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Application.Database;

public interface IBaseRepository<TEntity, TKey>
           where TEntity : IEntity<TKey>
           where TKey : IEntityId
{
    Task Add(TEntity entity, CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAsync(Func<TEntity>? filters = null, Func<TEntity>? order = null, PagedParameters? paging = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Func<TEntity>? predicate = null, CancellationToken cancellationToken = default);

    Task Update(TEntity entity, CancellationToken cancellationToken = default);

    Task Delete(TEntity entity, CancellationToken cancellationToken = default);
}
