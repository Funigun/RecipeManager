using RecipeManager.API.Domain.Common.Abstractions;
using RecipeManager.Api.Shared.Contracts.Exceptions;

namespace RecipeManager.API.Application.Exceptions;

public sealed class EntityNotFoundException<TEntity, TId> : NotFoundException
              where TEntity : class, IEntity<TId>
              where TId : notnull
{
    public EntityNotFoundException(TId id) : base($"Entity of type '{typeof(TEntity).Name}' with ID '{id}' was not found.")
    {
    }
}
