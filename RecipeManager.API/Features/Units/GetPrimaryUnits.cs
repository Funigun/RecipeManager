using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Units;

public static class GetPrimaryUnits
{
    public sealed record Response(Guid UnitId, string Name);

    [GroupEndpoint("Units")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("/primary-units", Handler)
                     .WithName("GetPrimaryUnits")
                     .WithDescription("Returns list of primary measurement units");

        }
    }

    internal static async Task<Results<Ok<IEnumerable<Response>>, BadRequest>> Handler([FromServices] IUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        IEnumerable<Unit> primaryUnits = await unitOfWork.Units.GetPrimaryUnitsAsync(cancellationToken);

        IEnumerable<Response> response = primaryUnits.Select(u => new Response(u.Id, u.Name));

        return TypedResults.Ok(response);
    }
}
