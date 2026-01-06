using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Domain.Units.Enums;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Persistance.Migrations;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using RecipeManager.Shared.Contracts.Authorization;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.Units;

public static class GetUnits
{
    public sealed record PrimaryUnitDto(Guid Id, string Name, int ConversionFactory);

    public sealed record Response(
        Guid UnitId,
        string Name,
        string? ShortName,
        string PluralName,
        string? PluralShortName,
        string Group,
        bool IsBaseUnit,
        PrimaryUnitDto? PrimaryUnit
    );

    [GroupEndpoint("Units")]
    public class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>(string.Empty, Handler)
                     .WithName("GetMeasurementUnits")
                     .WithDescription("Gets all measurement units");
        }
    }

    internal static async Task<Results<Ok<HateoasCollectionResponse<Response>>, NotFound>> Handler([FromServices] ICurrentUser currentUser, [FromServices] IUnitOfWork unitOfWork, [FromServices] IHateoasBuilderFactory hateoasBuilderFactory, CancellationToken cancellationToken)
    {
        IEnumerable<Unit> units = await unitOfWork.Units.GetAsync(null, q => q.DefaultOrder(), null, cancellationToken);

        IEnumerable<Response> responses = units.Select(unit => unit.ToGetResponse(units));

        bool isActionAllowed = currentUser.HasRole(UserRoles.Admin);

        HateoasCollectionResponseBuilder<Response>? responseBuilder = hateoasBuilderFactory.ForCollection(responses);

        responseBuilder.WithCollectionLink()
                        .WithGet(LinkOptions.Create("GetMeasurementUnit", HateoasRelConstants.Self, isActionAllowed), unit => new { unitId = unit.UnitId })
                        .WithPut(LinkOptions.Create("UpdateMeasurementUnit", HateoasRelConstants.Update, isActionAllowed), unit => new { unitId = unit.UnitId })
                        .WithDelete(LinkOptions.Create("DeleteMeasurementUnit", HateoasRelConstants.Delete, isActionAllowed), unit => new { unitId = unit.UnitId })
                      .AddPost(LinkOptions.Create("GetMeasurementUnits", HateoasRelConstants.Create, isActionAllowed), null);

        return TypedResults.Ok(responseBuilder.Build());
    }

    internal static Response ToGetResponse(this Unit unit, IEnumerable<Unit> units)
    {
        Unit? primaryUnit = unit.PrimaryUnit == null ? null : units.First(u => u.Id == unit.PrimaryUnit);

        return new Response
        (
            unit.Id.Value,
            unit.Name,
            unit.ShortName,
            unit.PluralName,
            unit.PluralShortName,
            unit.Group.ToFriendlyString(),
            unit.IsBaseUnit,
            primaryUnit is not null ? new PrimaryUnitDto(primaryUnit.Id.Value, primaryUnit.Name, unit.ConversionFactor) : null
        );
    }
}
