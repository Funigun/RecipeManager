using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Units;

public static class DeleteUnit
{
    public sealed record Request(UnitId UnitId);

    public sealed class AuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public Task<bool> IsAuthorized(Request request)
        {
            return Task.FromResult(currentUser.HasRole(UserRoles.Admin));
        }
    }

    [GroupEndpoint("Units")]
    public class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedDelete<Request>(string.Empty, Handler)
                     .WithName("DeleteMeasurementUnit")
                     .WithDescription("Deletes a measurement unit");
        }
    }

    internal static async Task<Results<NoContent, NotFound>> Handler(Request request, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        Unit? unit = await dbContext.Units.FindAsync([request.UnitId], cancellationToken) ?? throw new EntityNotFoundException<Unit, UnitId>(request.UnitId);

        dbContext.Units.Remove(unit);

        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
