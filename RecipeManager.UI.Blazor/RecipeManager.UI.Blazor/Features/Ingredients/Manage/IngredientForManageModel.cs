using System.Collections.ObjectModel;
using RecipeManager.UI.Blazor.Features.IngredientCategories.Models;
using RecipeManager.UI.Blazor.Features.Recipes;

namespace RecipeManager.UI.Blazor.Features.Ingredients.Manage;

public sealed class IngredientForManageModel
{
    public string Name { get; set; } = string.Empty;

    public IngredientCategoryForDropdownModel ShoppingListCategory { get; set; } = new();

    public Guid? BaseUnit { get; set; }

    public NutritionalValuesModel NutritionalValues { get; set; } = new();

    public IngredientPackageModel IngredientPackage { get; set; } = new();

    public Collection<IngredientCategoryForDropdownModel> Categories { get; set; } = [];

    public Collection<RecipeForDropdownModel> Recipes { get; set; } = [];

    public Collection<IngredientUnitConvertionModel> Convertions { get; set; } = [];

    public IngredientForUpdateModel ToUpdateModel()
    {
        return new IngredientForUpdateModel
        {
            Name = Name,
            NutritionalValues = NutritionalValues,
            ShoppingListCategoryId = ShoppingListCategory.Id,
            Categories = new(Categories.Select(c => c.Id).ToList()),
            Recipes = new(Recipes.Select(r => r.Id).ToList())
        };
    }

    public IngredientForCreateModel ToCreateModel()
    {
        return new IngredientForCreateModel
        {
            Name = Name,
            NutritionalValues = NutritionalValues,
            ShoppingListCategoryId = ShoppingListCategory.Id,
            Categories = new(Categories.Select(c => c.Id).ToList()),
            Recipes = new(Recipes.Select(r => r.Id).ToList())
        };
    }
}
