namespace RecipeManager.UI.Blazor.Features.Cookbooks.Models;

public sealed class CookbookCategoryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public IEnumerable<CookbookCategoryDto> Subcategories { get; set; } = [];

    public IEnumerable<CookbookRecipeDto> Recipes { get; set; } = [];
}
