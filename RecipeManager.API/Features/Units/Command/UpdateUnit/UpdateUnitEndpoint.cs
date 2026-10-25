using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Units.Command.UpdateUnit;

public sealed class UpdateUnitEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardPut<UpdateUnitRequest>(UnitsGroup.UnitByIdRoute, Handle)
                 .WithName("UpdateMeasurementUnit")
                 .WithDescription("Updates a measurement unit")
                 .WithTags(UnitsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status404NotFound)
                 .ProducesProblem(StatusCodes.Status403Forbidden)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> Handle(
        Guid unitId,
        UpdateUnitRequest request,
        [FromServices] UpdateUnitCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        UpdateUnitCommand command = new(unitId, request);
        Result<UnitId> result = await commandHandler.Handle(command, cancellationToken);
        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}
