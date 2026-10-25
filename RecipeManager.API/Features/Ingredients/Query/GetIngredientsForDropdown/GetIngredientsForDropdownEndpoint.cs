using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Ingredients.Query.GetIngredientsForDropdown;

public sealed class GetIngredientsForDropdownEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetIngredientsForDropdownResponse>(IngredientsGroup.DropdownRoute, Handle)
                 .WithName("GetIngredientsForDropdown")
                 .WithDescription("Gets a list of ingredients for use in dropdowns.")
                 .WithTags(IngredientsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Ok<IEnumerable<GetIngredientsForDropdownResponse>>> Handle(
        [FromQuery] string? ingredientName,
        [FromServices] GetIngredientsForDropdownQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetIngredientsForDropdownResponse> result = await queryHandler.Handle(
            new GetIngredientsForDropdownQuery(ingredientName),
            cancellationToken);
        return TypedResults.Ok(result);
    }
}
