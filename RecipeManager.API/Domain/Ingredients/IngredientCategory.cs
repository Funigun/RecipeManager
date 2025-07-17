namespace RecipeManager.Api.Domain.Ingredients;

public sealed class IngredientCategory
{
    public IngredientCategoryId Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
