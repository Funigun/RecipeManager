using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.RecipeCategories.Query.GetRecipeCategoriesForDropdown;

public sealed class GetRecipeCategoriesForDropdownEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetRecipeCategoriesForDropdownResponse>(RecipeCategoriesGroup.DropdownRoute, Handle)
                 .WithName("GetRecipeCategoriesDropdown")
                 .WithDescription("Gets all recipe categories")
                 .WithTags(RecipeCategoriesGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Ok<IEnumerable<GetRecipeCategoriesForDropdownResponse>>> Handle(
        [FromQuery] string? categoryName,
        [FromServices] GetRecipeCategoriesForDropdownQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetRecipeCategoriesForDropdownResponse> response = await queryHandler.Handle(
            new GetRecipeCategoriesForDropdownQuery(
                new GetRecipeCategoriesForDropdownRequest(categoryName)), cancellationToken);
        return TypedResults.Ok(response);
    }
}
