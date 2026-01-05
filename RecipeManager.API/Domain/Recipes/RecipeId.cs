using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Recipes;

public record RecipeId(Guid Value) : IEntityId
{
    public static implicit operator Guid(RecipeId id) => id.Value;

    public static implicit operator RecipeId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
