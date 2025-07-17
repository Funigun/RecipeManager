using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Ingredients;

public sealed class Ingredient : AuditableEntity, IEntity<IngredientId>
{
    public IngredientId Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
