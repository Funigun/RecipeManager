using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Units;

public static class DeleteUnit
{
    public record struct Request(Guid Id) : IRequestId<Request>
    {
    }

    public sealed class AuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<Request>
    {
        public Task<bool> IsAuthorized(Request request)
        {
            return Task.FromResult(currentUser.HasRole(UserRoles.Admin));
        }
    }

    [GroupEndpoint("Units")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedDelete<AuthorizationPolicy, Request>("/{unitId}", Handler)
                     .WithName("DeleteMeasurementUnit")
                     .WithDescription("Deletes a measurement unit");
        }
    }

    public static async Task<Results<NoContent, NotFound>> Handler(Request unitId, [FromServices] IUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        UnitId id = new(unitId.Id);

        Unit? unit = await unitOfWork.Units.GetByIdAsync(id, cancellationToken)
                  ?? throw new EntityNotFoundException<Unit, UnitId>(id);

        await unitOfWork.Units.Delete(unit, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
