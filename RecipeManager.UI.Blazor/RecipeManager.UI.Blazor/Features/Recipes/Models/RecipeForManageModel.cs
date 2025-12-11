using System.Collections.ObjectModel;
using RecipeManager.UI.Blazor.Features.Ingredients;
using RecipeManager.UI.Blazor.Features.RecipeCategories.Models;

namespace RecipeManager.UI.Blazor.Features.Recipes.Models;

public sealed class RecipeForManageModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public string? VideoUrl { get; set; }

    public RecipeAmountModel Amount { get; set; } = new();

    public byte NumberOfServings { get; set; }

    public RecipeDifficulty Difficulty { get; set; }

    public NutritionalValuesModel NutritionalValues { get; set; } = new();

    public ICollection<RecipeIngredientModel> Ingredients { get; set; } = [];

    public ICollection<RecipeSectionModel> Sections { get; set; } =
    [
        new(RecipeSectionType.PreCooking, [], false),
        new(RecipeSectionType.IngredientsPreparation, [], true),
        new(RecipeSectionType.Cooking, [], true),
        new(RecipeSectionType.Serving, [], false)
    ];

    public Collection<RecipeCategoryForDropdownModel> Categories { get; set; } = [];

    public IngredientForDropdownModel? Ingredient { get; set; }
}
