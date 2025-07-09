using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Domain.Units.Enums;
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
    public sealed record Response(Guid UnitId, string Name, string? ShortName, string Group);

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

    internal static async Task<Results<Ok<HateoasCollectionResponse<Response>>, NotFound>> Handler(ICurrentUser currentUser, IAppDbContext dbContext, HateoasBuilder<HateoasCollectionResponse<Response>> hateoasBuilder, CancellationToken cancellationToken)
    {
        IEnumerable<Unit> units = await dbContext.Units.AsNoTracking().ToListAsync(cancellationToken);
        IEnumerable<Response> responses = units.Select(ToGetResponse);

        bool isActionAllowed = currentUser.HasRole(UserRoles.Admin);

        hateoasBuilder.ForCollection(responses)
                      .WithCollectionLink()
                        .WithDelete(LinkOptions.Create("DeleteMeasurementUnit", HateoasRelConstants.Delete, isActionAllowed), unit => new { unitId = unit.UnitId })
                        .WithPut(LinkOptions.Create("UpdateMeasurementUnit", HateoasRelConstants.Update, isActionAllowed), unit => new { unitId = unit.UnitId })
                      .AddPost(LinkOptions.Create("GetMeasurementUnits", HateoasRelConstants.Create, isActionAllowed), null);

        return TypedResults.Ok(hateoasBuilder.BuildResults());
    }

    internal static Response ToGetResponse(this Unit unit)
    {
        return new Response
        (
            unit.Id.Value,
            unit.Name,
            unit.ShortName,
            unit.Group.ToFriendlyString()
        );
    }
}
