using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Units;

public static class DeleteUnit
{
    public sealed class AuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<Guid>
    {
        public Task<bool> IsAuthorized(Guid request)
        {
            return Task.FromResult(currentUser.HasRole(UserRoles.Admin));
        }
    }

    [GroupEndpoint("Units")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedDelete<AuthorizationPolicy, Guid>("/{unitId}", Handler)
                     .WithName("DeleteMeasurementUnit")
                     .WithDescription("Deletes a measurement unit");
        }
    }

    public static async Task<Results<NoContent, NotFound>> Handler(Guid unitId, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        UnitId id = new(unitId);

        Unit? unit = await dbContext.Units.FindAsync([id], cancellationToken)
                  ?? throw new EntityNotFoundException<Unit, UnitId>(id);

        dbContext.Units.Remove(unit);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
