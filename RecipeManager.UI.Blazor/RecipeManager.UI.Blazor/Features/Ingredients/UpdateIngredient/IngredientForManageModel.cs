using System.Collections.ObjectModel;
using RecipeManager.UI.Blazor.Features.IngredientCategories.Models;
using RecipeManager.UI.Blazor.Features.Recipes;

namespace RecipeManager.UI.Blazor.Features.Ingredients.UpdateIngredient;

public sealed class IngredientForManageModel
{
    public string Name { get; set; } = string.Empty;

    public Collection<IngredientCategoryForDropdownModel> Categories { get; set; } = [];

    public Collection<RecipeForDropdownModel> Recipes { get; set; } = [];

    public IngredientForUpdateModel ToUpdateModel()
    {
        return new IngredientForUpdateModel
        {
            Name = Name,
            Categories = new(Categories.Select(c => c.Id).ToList()),
            Recipes = new(Recipes.Select(r => r.Id).ToList())
        };
    }
}
