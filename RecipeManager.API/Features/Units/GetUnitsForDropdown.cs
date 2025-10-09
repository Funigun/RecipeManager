using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
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

    private static async Task<Results<Ok<IEnumerable<Response>>, NotFound>> Handler([FromServices] IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        IEnumerable<Response> results = await dbContext.Units
                                                       .AsNoTracking()
                                                       .OrderBy(unit => unit.Group)
                                                         .ThenBy(unit => unit.PrimaryUnit == null ? 1 : 2)
                                                         .ThenBy(unit => unit.PrimaryUnit != null ? unit.ConversionFactor : 1000)
                                                         .ThenBy(unit => unit.Name)
                                                       .Select(unit => new Response(unit.Id, unit.ShortName ?? unit.Name))
                                                       .ToListAsync(cancellationToken);

        return TypedResults.Ok(results);
    }
}
