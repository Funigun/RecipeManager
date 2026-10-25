using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.IngredientCategories.Query.GetIngredientCategoriesForDropdown;

public sealed class GetIngredientCategoriesForDropdownEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetIngredientCategoriesForDropdownResponse>(
                     IngredientCategoryGroup.DropdownRoute, Handle)
                 .WithName("GetIngredientCategoriesDropdown")
                 .WithDescription("Gets all ingredient categories")
                 .WithTags(IngredientCategoryGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Ok<IEnumerable<GetIngredientCategoriesForDropdownResponse>>> Handle(
        [FromQuery] string? categoryName,
        [FromServices] GetIngredientCategoriesForDropdownQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetIngredientCategoriesForDropdownResponse> response = await queryHandler.Handle(
            new GetIngredientCategoriesForDropdownQuery(
                new GetIngredientCategoriesForDropdownRequest(categoryName)), cancellationToken);
        return TypedResults.Ok(response);
    }
}
