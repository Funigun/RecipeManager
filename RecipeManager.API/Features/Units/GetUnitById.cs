using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
    public sealed record Response(Guid UnitId, string Name, string? ShortName, string PluralName, string? PluralShortName, int Group, bool IsBaseUnit, Guid? PrimaryUnit, int ConversionFactor);

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

    internal static async Task<Results<Ok<HateoasResponse<Response>>, NotFound>> Handler(Guid unitId, [FromServices] ICurrentUser currentUser, [FromServices] IAppDbContext dbContext, [FromServices] IHateoasBuilderFactory hateoasBuilderFactory, CancellationToken cancellationToken)
    {
        UnitId id = new(unitId);
        Unit? unit = await dbContext.Units.AsNoTracking().FirstOrDefaultAsync(unit => unit.Id == id, cancellationToken);

        if (unit is null)
        {
            throw new EntityNotFoundException<Unit, UnitId>(new(unitId));
        }

        bool isActionAllowed = currentUser.HasRole(UserRoles.Admin);

        HateoasResponseBuilder<Response>? responseBuilder = hateoasBuilderFactory.ForItem(unit.ToSingleGetResponse());

        responseBuilder.AddDelete(LinkOptions.Create("DeleteMeasurementUnit", HateoasRelConstants.Delete, isActionAllowed), new { unitId = unit.Id })
                       .AddPut(LinkOptions.Create("UpdateMeasurementUnit", HateoasRelConstants.Update, isActionAllowed), new { unitId = unit.Id });

        return TypedResults.Ok(responseBuilder.Build());
    }

    internal static Response ToSingleGetResponse(this Unit unit)
    {
        return new Response
        (
            unit.Id.Value,
            unit.Name,
            unit.ShortName,
            unit.PluralName,
            unit.PluralShortName,
            (int)unit.Group,
            unit.IsBaseUnit,
            unit.PrimaryUnit?.Value ?? null,
            unit.ConversionFactor
        );
    }
}
