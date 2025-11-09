using System.Text.Json.Serialization;

namespace RecipeManager.UI.Blazor.Features.Cookbooks.Models;

public class CookbookCategoryForManageModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public bool IsSelected { get; set; } = false;

    [JsonIgnore]
    public int DepthLevel { get; set; } = 1;

    public List<CookbookRecipeForManageModel> Recipes { get; set; } = [];

    public List<CookbookCategoryForManageModel> Subcategories { get; set; } = [];

    public void ResetSelection()
    {
        IsSelected = false;

        foreach (CookbookCategoryForManageModel subcategory in Subcategories)
        {
            subcategory.ResetSelection();
        }
    }
}
