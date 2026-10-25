using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Presentation;

namespace RecipeManager.Api.Features.Units.Query.GetUnitById;

public sealed class GetUnitByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetUnitByIdResponse>(UnitsGroup.UnitByIdRoute, Handle)
                 .WithName("GetMeasurementUnit")
                 .WithDescription("Gets a measurement unit by its id")
                 .WithTags(UnitsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status404NotFound)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Ok<GetUnitByIdResponse>, ValidationProblem, ProblemHttpResult>> Handle(
        Guid unitId,
        [FromServices] GetUnitByIdQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        GetUnitByIdQuery query = new(unitId);
        Result<GetUnitByIdResponse> result = await queryHandler.Handle(query, cancellationToken);
        return result.ToHttpResult(response => TypedResults.Ok(response));
    }
}
