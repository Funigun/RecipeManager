using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using RecipeManager.API.Application.Abstractions;
using RecipeManager.API.Domain.Units;
using RecipeManager.API.Domain.Units.Enums;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Authorization;
using RecipeManager.Shared.Contracts.Units;

namespace RecipeManager.API.Features.Units;

public static class CreateUnit
{
    public sealed record Request(string Name, string? ShortName, int Group);

    public sealed record Response(UnitId Id);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).SetValidator(new UnitNameValidator());

            RuleFor(x => x.ShortName).SetValidator(new UnitShortNameValidator());

            RuleFor(x => x.Group)
                .Must(group => group.IsUnitGroup())
                    .WithMessage("Invalid unit group")
                    .WithName("Unit Group");
        }
    }

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
            endpoints.MapStandardAuthenticatedPost<Request, Response>(string.Empty, Handler)
                     .WithName("CreateMeasurementUnit")
                     .WithDescription("Creates new measurement unit");
        }
    }

    internal static async Task<Results<Ok<Response>, NotFound>> Handler(Request request, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        Unit unit = request.ToUnit();

        dbContext.Units.Add(unit);

        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(unit.ToResponse());
    }

    private static Unit ToUnit(this Request request)
    {
        return Unit.Create(request.Name, request.ShortName, (UnitGroup)request.Group);
    }

    private static Response ToResponse(this Unit unit)
    {
        return new Response(unit.Id);
    }
}
