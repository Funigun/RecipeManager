namespace RecipeManager.UI.Blazor.Features.Cookbooks.GetCookbooks;

public class CookbookModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string CoverImageUrl { get; set; } = string.Empty;
}
