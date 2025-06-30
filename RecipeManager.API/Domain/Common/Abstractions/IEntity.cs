namespace RecipeManager.API.Domain.Common.Abstractions;

public interface IEntity<TEntityId>
{
    TEntityId Id { get; set; }
}
