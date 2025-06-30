namespace RecipeManager.API.Domain.Common.Abstractions;

public interface IEntity
{
}

public interface IEntity<TEntityId> : IEntity
{
    TEntityId Id { get; set; }
}
