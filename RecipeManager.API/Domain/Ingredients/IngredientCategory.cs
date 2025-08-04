using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Ingredients;

public sealed class IngredientCategory : AuditableEntity, IEntity<IngredientCategoryId>
{
    public IngredientCategoryId Id { get; set; } = default!;

    public string Name { get; set; } = string.Empty;

    private IngredientCategory()
    {
    }

    public static IngredientCategory Create(string name)
    {
        return new()
        {
            Name = name
        };
    }
}
