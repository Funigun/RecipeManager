using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Units;

public static class GetUnitsForDropdown
{
    public sealed record Response(Guid Id, string Name);

    [GroupEndpoint("Units")]
    public class Endponint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("/dropdown", Handler)
                     .WithName("GetUnitsForDropdown")
                     .WithDescription("Gets all measurement units for dropdown selection (Create/Update recipe)");
        }
    }

    public static async Task<Results<Ok<IEnumerable<Response>>, NotFound>> Handler([FromServices] IUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        IEnumerable<Unit> units = await unitOfWork.Units.GetAsync(null, q => q.DefaultOrder(), null, cancellationToken);

        IEnumerable<Response> results = units.Select(unit => new Response(unit.Id, unit.ShortName ?? unit.Name));

        return TypedResults.Ok(results);
    }
}
