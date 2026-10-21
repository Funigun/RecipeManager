namespace RecipeManager.Api.Features.Cookbooks;

public static class CookbooksGroup
{
    public const string Route = "/api/cookbooks";
    public const string Tag = "Cookbooks";
    public const string CookbookByIdRoute = $"{Route}/{{cookbookId}}";

    public static string CookbookById(Guid cookbookId) => $"{Route}/{cookbookId}";
}
