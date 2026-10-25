using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Units.Command.CreateUnit;

public sealed class CreateUnitEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardPost<CreateUnitRequest, CreateUnitResponse>(UnitsGroup.Route, Handle)
                 .WithName("CreateMeasurementUnit")
                 .WithDescription("Creates new measurement unit")
                 .WithTags(UnitsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status403Forbidden)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Created<CreateUnitResponse>, ValidationProblem, ProblemHttpResult>> Handle(
        CreateUnitRequest request,
        [FromServices] CreateUnitCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        CreateUnitCommand command = new(request);
        Result<CreateUnitResponse> result = await commandHandler.Handle(command, cancellationToken);

        return result.ToHttpResult(response => TypedResults.Created(UnitsGroup.UnitById(response.Id.Value), response));
    }
}
