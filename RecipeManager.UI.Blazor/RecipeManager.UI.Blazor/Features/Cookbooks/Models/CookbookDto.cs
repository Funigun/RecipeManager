namespace RecipeManager.UI.Blazor.Features.Cookbooks.Models;

public sealed class CookbookDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? CoverImageUrl { get; set; }

    public IEnumerable<CookbookCategoryDto> Categories { get; set; } = [];

    public static CookbookDto FromManageModel(CookbookForManageModel model)
    {
        return new CookbookDto
        {
            Id = model.Id,
            Title = model.Title,
            Description = model.Description,
            CoverImageUrl = model.CoverImageUrl,
            Categories = model.Categories.Select(MapCategory).ToList()
        };
    }

    private static CookbookCategoryDto MapCategory(CookbookCategoryForManageModel category)
    {
        return new CookbookCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Subcategories = category.Subcategories.Select(MapCategory).ToList(),
            Recipes = category.Recipes.Select(r => new CookbookRecipeDto { Id = r.Item.Id })
                                      .ToList()
        };
    }
}
