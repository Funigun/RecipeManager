using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;

namespace RecipeManager.UI.Blazor.Features.Cookbooks.Models;

public class CookbookCategoryForManageModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public bool IsSelected { get; set; } = false;

    [JsonIgnore]
    public int DepthLevel { get; set; } = 1;

    public Collection<HateoasResponse<CookbookRecipeForManageModel>> Recipes { get; set; } = [];

    public Collection<CookbookCategoryForManageModel> Subcategories { get; set; } = [];

    public void ResetSelection()
    {
        IsSelected = false;

        foreach (CookbookCategoryForManageModel subcategory in Subcategories)
        {
            subcategory.ResetSelection();
        }
    }

    public IEnumerable<HateoasResponse<CookbookRecipeForManageModel>> GetAllRecipes()
    {
        List<HateoasResponse<CookbookRecipeForManageModel>> recipes = Recipes.ToList();
        recipes.AddRange(Subcategories.SelectMany(s => s.GetAllRecipes()));

        return recipes;
    }

    public bool CategoryExists(string categoryName)
    {
        if (Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        foreach (CookbookCategoryForManageModel subcategory in Subcategories)
        {
            if (subcategory.CategoryExists(categoryName))
            {
                return true;
            }
        }

        return false;
    }

    public bool DeleteSubcategory(CookbookCategoryForManageModel categoryToRemove)
    {
        if (categoryToRemove.DepthLevel - DepthLevel == 1)
        {
            if (Subcategories.Remove(categoryToRemove))
            {
                return true;
            }
            else
            {
                foreach (CookbookCategoryForManageModel subcategory in Subcategories)
                {
                    if (subcategory.DeleteSubcategory(categoryToRemove))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
