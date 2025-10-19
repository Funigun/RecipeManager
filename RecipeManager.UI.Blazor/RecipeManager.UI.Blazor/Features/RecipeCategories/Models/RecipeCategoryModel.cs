namespace RecipeManager.UI.Blazor.Features.RecipeCategories.Models;

public sealed class RecipeCategoryModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;
}
