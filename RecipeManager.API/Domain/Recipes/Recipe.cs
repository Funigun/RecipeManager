using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Common.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Domain.Recipes.ValueObjects;

namespace RecipeManager.Api.Domain.Recipes;

public sealed class Recipe : AuditableEntity, IEntity<RecipeId>
{
    private List<RecipeCategoryId> _categories = [];

    public RecipeId Id { get; set; } = default!;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageURL { get; set; }

    public string? VideoURL { get; set; }

    public RecipeAmount Amount { get; set; } = default!;

    public byte NumberOfServings { get; set; }

    public RecipeDifficulty Difficulty { get; set; }

    public NutritionalValue NutritionalValue { get; set; }

    public IngredientId? IngredientId { get; set; }

    public ICollection<RecipeIngredient> Ingredients { get; set; } = [];

    public ICollection<RecipeSection> Sections { get; set; } = [];

    public IReadOnlyList<RecipeCategoryId> Categories => _categories.ToList();

    private Recipe()
    {
    }

    public static Recipe Create(string title, RecipeAmount amount, byte numberOfServings, RecipeDifficulty difficulty, NutritionalValue nutritionalValue, IEnumerable<RecipeIngredient> ingredients, IEnumerable<RecipeSection> sections, IEnumerable<RecipeCategoryId> categories, IngredientId ingredientId, string? description = null, string? imageUrl = null, string? videoUrl = null)
    {
        return new()
        {
            Title = title,
            Amount = amount,
            NumberOfServings = numberOfServings,
            Difficulty = difficulty,
            NutritionalValue = nutritionalValue,
            IngredientId = ingredientId,
            Ingredients = ingredients.ToList(),
            Sections = sections.ToList(),
            _categories = categories.ToList(),
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

    public void UpdateCategories(IEnumerable<RecipeCategoryId> categoryIds)
    {
        _categories = categoryIds.ToList();
    }

    public void UpdateIngredients(IEnumerable<RecipeIngredient> ingredients)
    {
        IEnumerable<RecipeIngredient> ingredientsToRemove = Ingredients.Where(ingredient => !ingredients.Any(i => i.Id == ingredient.Id));
        Ingredients = Ingredients.Except(ingredientsToRemove).ToList();

        foreach (RecipeIngredient ingredient in ingredients)
        {
            RecipeIngredient? existingIngredient = Ingredients.FirstOrDefault(ingr => ingr.Id == ingredient.Id);

            if (existingIngredient is not null)
            {
                existingIngredient.UnitId = ingredient.UnitId;
                existingIngredient.Amount = ingredient.Amount;
            }
            else
            {
                Ingredients.Add(ingredient);
            }
        }
    }

    public void UpdateSections(IEnumerable<RecipeSection> sections)
    {
        foreach (RecipeSection updatedSection in sections)
        {
            RecipeSection? existingSection = Sections.FirstOrDefault(section => section.Type == updatedSection.Type);

            if (existingSection is not null)
            {
                existingSection.UpdateSection(updatedSection);
            }
            else
            {
                Sections.Add(updatedSection);
            }
        }
    }

    public int CalculateCaloriesForSingleServing()
    {
        return (int)Math.Round((decimal)(NutritionalValue.Calories / NumberOfServings), 0);
    }

    public double CalculateProteinsForSingleServing()
    {
        return Math.Round(NutritionalValue.Proteins / NumberOfServings, 2);
    }

    public double CalculateFatsForSingleServing()
    {
        return Math.Round(NutritionalValue.Fats / NumberOfServings, 2);
    }

    public double CalculateCarbohydratesForSingleServing()
    {
        return Math.Round(NutritionalValue.Carbohydrates / NumberOfServings, 2);
    }
}
