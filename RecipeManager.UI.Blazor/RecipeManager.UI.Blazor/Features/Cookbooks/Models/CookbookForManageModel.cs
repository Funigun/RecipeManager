using RecipeManager.UI.Blazor.Brokers.HateoasModel;

namespace RecipeManager.UI.Blazor.Features.Cookbooks.Models;

public sealed class CookbookForManageModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? CoverImageUrl { get; set; }

    public List<CookbookCategoryForManageModel> Categories { get; set; } = new();

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
}
