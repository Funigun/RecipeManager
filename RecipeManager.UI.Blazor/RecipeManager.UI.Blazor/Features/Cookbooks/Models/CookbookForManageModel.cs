using RecipeManager.UI.Blazor.Brokers.HateoasModel;

namespace RecipeManager.UI.Blazor.Features.Cookbooks.Models;

public sealed class CookbookForManageModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? CoverImageUrl { get; set; }

    public List<CookbookCategoryForManageModel> Categories { get; set; } = [];

    public void ResetCategorySelection()
    {
        foreach (CookbookCategoryForManageModel category in Categories)
        {
            category.ResetSelection();
        }
    }

    public IEnumerable<HateoasResponse<CookbookRecipeForManageModel>> GetRecipes()
    {
        return Categories.SelectMany(category => category.GetAllRecipes()).ToList();
    }

    public bool CategoryExists(string categoryName)
    {
        return Categories.Any(category => category.CategoryExists(categoryName));
    }

    public void DeleteCategory(CookbookCategoryForManageModel categoryToRemove)
    {
        if (categoryToRemove.DepthLevel == 1)
        {
            Categories.Remove(categoryToRemove);
        }
        else if (categoryToRemove.DepthLevel == 2)
        {
            foreach (CookbookCategoryForManageModel category in Categories)
            {
                if (category.Subcategories.Remove(categoryToRemove))
                {
                    break;
                }
            }
        }
        else
        {
            foreach (CookbookCategoryForManageModel category in Categories)
            {
                if (category.DeleteSubcategory(categoryToRemove))
                {
                    break;
                }
            }
        }
    }
}
