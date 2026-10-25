using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Units.Command.DeleteUnit;

public sealed class DeleteUnitEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardDelete<DeleteUnitCommand>(UnitsGroup.UnitByIdRoute, Handle)
                 .WithName("DeleteMeasurementUnit")
                 .WithDescription("Deletes a measurement unit")
                 .WithTags(UnitsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status404NotFound)
                 .ProducesProblem(StatusCodes.Status403Forbidden)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> Handle(
        Guid unitId,
        [FromServices] DeleteUnitCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<UnitId> result = await commandHandler.Handle(new DeleteUnitCommand(unitId), cancellationToken);
        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}
