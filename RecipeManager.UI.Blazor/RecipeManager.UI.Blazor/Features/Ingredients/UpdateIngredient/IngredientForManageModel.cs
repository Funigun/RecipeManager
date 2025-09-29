using RecipeManager.UI.Blazor.Features.IngredientCategories.Models;
using RecipeManager.UI.Blazor.Features.Recipes;

namespace RecipeManager.UI.Blazor.Features.Ingredients.UpdateIngredient;

public sealed class IngredientForManageModel
{
    public string Name { get; set; }

    public IEnumerable<IngredientCategoryForDropdownModel> Categories { get; set; }

    public IEnumerable<RecipeForDropdownModel> Recipes { get; set; }

    public IngredientForUpdateModel ToUpdateModel()
    {
        return new IngredientForUpdateModel
        {
            Name = Name,
            Categories = Categories.Select(c => c.Id).ToList(),
            Recipes = Recipes.Select(r => r.Id).ToList()
        };
    }
}
