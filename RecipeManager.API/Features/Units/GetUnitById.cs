using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using RecipeManager.Shared.Contracts.Authorization;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.Units;

public static class GetUnitById
{
    public sealed record Response(Guid UnitId, string Name, string? ShortName, int Group);

    [GroupEndpoint("Units")]
    public class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("/{unitId}", Handler)
                     .WithName("GetMeasurementUnit")
                     .WithDescription("Gets all measurement units");
        }
    }

    internal static async Task<Results<Ok<HateoasResponse<Response>>, NotFound>> Handler(Guid unitId, ICurrentUser currentUser, IAppDbContext dbContext, HateoasBuilder<HateoasResponse<Response>> hateoasBuilder, CancellationToken cancellationToken)
    {
        UnitId id = new(unitId);
        Unit? unit = await dbContext.Units.AsNoTracking().FirstOrDefaultAsync(unit => unit.Id == id, cancellationToken);

        if (unit is null)
        {
            throw new EntityNotFoundException<Unit, UnitId>(new(unitId));
        }

        bool isActionAllowed = currentUser.HasRole(UserRoles.Admin);

        hateoasBuilder.ForItem(unit.ToSingleGetResponse())
                        .AddDelete(LinkOptions.Create("DeleteMeasurementUnit", HateoasRelConstants.Delete, isActionAllowed), new { unitId = unit.Id })
                        .AddPut(LinkOptions.Create("UpdateMeasurementUnit", HateoasRelConstants.Update, isActionAllowed), new { unitId = unit.Id });

        return TypedResults.Ok(hateoasBuilder.BuildResults());
    }

    internal static Response ToSingleGetResponse(this Unit unit)
    {
        return new Response
        (
            unit.Id.Value,
            unit.Name,
            unit.ShortName,
            (int)unit.Group
        );
    }
}
