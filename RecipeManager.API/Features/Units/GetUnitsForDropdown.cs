using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Units;

public static class GetUnitsForDropdown
{
    public sealed record Response(Guid Id, string Name);

    public class Endponint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>(UnitsGroup.DropdownRoute, Handler)
                     .WithName("GetUnitsForDropdown")
                     .WithDescription("Gets all measurement units for dropdown selection (Create/Update recipe)")
                     .WithTags(UnitsGroup.Tag)
                     .RequireAuthorization("RecipeManagerPolicy")
                     .ProducesProblem(StatusCodes.Status401Unauthorized);
        }
    }

    public static async Task<Results<Ok<IEnumerable<Response>>, NotFound>> Handler([FromServices] IUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        IEnumerable<Unit> units = await unitOfWork.Units.GetAsync(null, q => q.DefaultOrder(), null, cancellationToken);

        IEnumerable<Response> results = units.Select(unit => new Response(unit.Id, unit.ShortName ?? unit.Name));

        return TypedResults.Ok(results);
    }
}
