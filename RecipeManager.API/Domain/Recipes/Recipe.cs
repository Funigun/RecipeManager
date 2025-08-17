using RecipeManager.Api.Domain.Common.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Domain.Recipes.ValueObjects;

namespace RecipeManager.Api.Domain.Recipes;

public sealed class Recipe : AuditableEntity, IEntity<RecipeId>
{
    public RecipeId Id { get; set; } = default!;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageURL { get; set; }

    public string? VideoURL { get; set; }

    public RecipeAmount Amount { get; set; } = default!;

    public byte NumberOfServings { get; set; }

    public RecipeDifficulty Difficulty { get; set; }

    public IngredientId? IngredientId { get; set; }

    public ICollection<RecipeIngredient> Ingredients { get; set; } = [];

    public ICollection<RecipeSection> Sections { get; set; } = [];

    public ICollection<RecipeCategory> Categories { get; set; } = [];

    private Recipe()
    {
    }

    public static Recipe Create(string title, RecipeAmount amount, byte numberOfServings, RecipeDifficulty difficulty, IEnumerable<RecipeIngredient> ingredients, IEnumerable<RecipeSection> sections, IEnumerable<RecipeCategory> categories, IngredientId ingredientId, string? description = null, string? imageUrl = null, string? videoUrl = null)
    {
        return new()
        {
            Title = title,
            Amount = amount,
            NumberOfServings = numberOfServings,
            Difficulty = difficulty,
            IngredientId = ingredientId,
            Ingredients = ingredients.ToList(),
            Sections = sections.ToList(),
            Categories = categories.ToList(),
            Description = description,
            ImageURL = imageUrl,
            VideoURL = videoUrl
        };
    }

    public IEnumerable<IngredientId> GetIngredientIds()
    {
        List<IngredientId> results = Ingredients.Select(ri => ri.IngredientId).ToList();

        if (IngredientId is not null)
        {
            results.Add(IngredientId);
        }

        return results;
    }
}
