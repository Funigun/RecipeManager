using RecipeManager.UI.Blazor.Features.Ingredients;

namespace RecipeManager.UI.Blazor.Features.Recipes.Models;

public sealed class RecipeModel
{
    public string Title { get; set; }

    public string Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? VideoUrl { get; set; }

    public RecipeAmountModel Amount { get; set; } = new();

    public byte NumberOfServings { get; set; }

    public RecipeDifficulty Difficulty { get; set; }

    public ICollection<RecipeIngredientModel> Ingredients { get; set; } = [];

    public ICollection<RecipeSectionModel> Sections { get; set; } =
    [
        new(RecipeSectionType.PreCooking, [], false),
        new(RecipeSectionType.IngredientsPreparation, [], true),
        new(RecipeSectionType.Cooking, [], true),
        new(RecipeSectionType.Serving, [], false)
    ];

    public List<Guid> Categories { get; set; } = [];

    public IngredientForDropdownModel? IngredientId { get; set; }
}
