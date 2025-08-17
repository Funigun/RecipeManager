using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Features.Recipes;

public static class GetRecipes
{
    public sealed record GetRecipesParameters(int PageNumber, int PageSize) : PagedParameters(PageNumber, PageSize);

    public sealed record Response(Guid Id, string Title, string? ImageUrl, int Difficulty, byte NumberOfServings);
}
