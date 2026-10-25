using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Units.Query.GetUnitsForDropdown;

public sealed class GetUnitsForDropdownEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetUnitsForDropdownResponse>(UnitsGroup.DropdownRoute, Handle)
                 .WithName("GetUnitsForDropdown")
                 .WithDescription("Gets all measurement units for dropdown selection (Create/Update recipe)")
                 .WithTags(UnitsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Ok<IEnumerable<GetUnitsForDropdownResponse>>> Handle(
        [FromServices] GetUnitsForDropdownQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetUnitsForDropdownResponse> result = await queryHandler.Handle(cancellationToken);
        return TypedResults.Ok(result);
    }
}
