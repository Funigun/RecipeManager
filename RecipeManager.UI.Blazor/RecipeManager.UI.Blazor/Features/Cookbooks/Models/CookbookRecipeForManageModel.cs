namespace RecipeManager.UI.Blazor.Features.Cookbooks.Models;

public class CookbookRecipeForManageModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
}
