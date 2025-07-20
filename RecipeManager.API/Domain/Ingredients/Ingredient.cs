using RecipeManager.Api.Domain.Common.Abstractions;
using RecipeManager.Api.Domain.Recipes;

namespace RecipeManager.Api.Domain.Ingredients;

public sealed class Ingredient : AuditableEntity, IEntity<IngredientId>
{
    private List<RecipeId> _recipes = [];

    private List<IngredientCategoryId> _categories = [];

    public IngredientId Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public IReadOnlyList<IngredientCategoryId> Categories => _categories.ToList();

    public IReadOnlyList<RecipeId> Recipes => _recipes.ToList();

    private Ingredient()
    {
    }

    public static Ingredient Create(string name, IEnumerable<IngredientCategoryId> categoryIds, IEnumerable<RecipeId> recipeIds)
    {
        return new()
        {
            Name = name,
            _categories = categoryIds.ToList(),
            _recipes = recipeIds.ToList()
        };
    }
}
