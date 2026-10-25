using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Units.Query.GetAllUnits;

public sealed class GetAllUnitsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetAllUnitsCollectionResponse>(UnitsGroup.Route, Handle)
                 .WithName("GetMeasurementUnits")
                 .WithDescription("Gets all measurement units")
                 .WithTags(UnitsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Ok<GetAllUnitsCollectionResponse>> Handle(
        [FromServices] GetAllUnitsQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        GetAllUnitsCollectionResponse result = await queryHandler.Handle(cancellationToken);
        return TypedResults.Ok(result);
    }
}
