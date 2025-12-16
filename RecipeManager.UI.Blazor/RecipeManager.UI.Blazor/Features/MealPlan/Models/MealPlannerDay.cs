using System.Text.Json.Serialization;
using RecipeManager.UI.Blazor.Features.Ingredients;
using RecipeManager.UI.Blazor.Features.Recipes;

namespace RecipeManager.UI.Blazor.Features.MealPlan.Models;

public class MealPlannerDay
{
    private readonly IEnumerable<Guid> _originalRecipeIds;

    public Guid Id { get; set; }

    [JsonIgnore]
    public Guid FrontId { get; set; }

    public DateTimeOffset Date { get; set; }

    public bool IsSelected { get; set; }

    public bool HasChanges => !_originalRecipeIds.SequenceEqual(Recipes.Select(r => r.Id));

    public ICollection<RecipeForMealPlanModel> Recipes { get; set; } = [];

    public NutritionalValuesModel NutritionalValues { get; set; } = new();

    public MealPlannerDay()
    {
        FrontId = Id == Guid.Empty ? Guid.CreateVersion7() : Id;
        _originalRecipeIds = Recipes.Select(r => r.Id);
    }

    public void AddRecipe(RecipeForMealPlanModel recipe)
    {
        RecipeForMealPlanModel newRecipe = recipe.CreateCopy();
        Recipes.Add(newRecipe);

        NutritionalValues.Calories += newRecipe.NutritionalValues.Calories;
        NutritionalValues.Carbohydrates += newRecipe.NutritionalValues.Carbohydrates;
        NutritionalValues.Fats += newRecipe.NutritionalValues.Fats;
        NutritionalValues.Proteins += newRecipe.NutritionalValues.Proteins;
    }

    public void RemoveRecipe(RecipeForMealPlanModel recipe)
    {
        Recipes.Remove(recipe);

        NutritionalValues.Calories -= recipe.NutritionalValues.Calories;
        NutritionalValues.Carbohydrates -= recipe.NutritionalValues.Carbohydrates;
        NutritionalValues.Fats -= recipe.NutritionalValues.Fats;
        NutritionalValues.Proteins -= recipe.NutritionalValues.Proteins;
    }

    public void RemoveSelectedRecipes()
    {
        List<RecipeForMealPlanModel> selectedRecipes = Recipes.Where(r => r.IsSelected).ToList();

        foreach (RecipeForMealPlanModel recipe in selectedRecipes)
        {
            RemoveRecipe(recipe);
        }
    }

    public void ClearRecipes()
    {
        Recipes.Clear();
        NutritionalValues.Calories = 0;
        NutritionalValues.Carbohydrates = 0;
        NutritionalValues.Fats = 0;
        NutritionalValues.Proteins = 0;
    }
}
