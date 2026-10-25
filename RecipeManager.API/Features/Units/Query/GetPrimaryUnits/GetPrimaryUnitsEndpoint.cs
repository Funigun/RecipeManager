using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Units.Query.GetPrimaryUnits;

public sealed class GetPrimaryUnitsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetPrimaryUnitsResponse>(UnitsGroup.PrimaryUnitsRoute, Handle)
                 .WithName("GetPrimaryUnits")
                 .WithDescription("Returns list of primary measurement units")
                 .WithTags(UnitsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Ok<IEnumerable<GetPrimaryUnitsResponse>>> Handle(
        [FromServices] GetPrimaryUnitsQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetPrimaryUnitsResponse> result = await queryHandler.Handle(cancellationToken);
        return TypedResults.Ok(result);
    }
}
