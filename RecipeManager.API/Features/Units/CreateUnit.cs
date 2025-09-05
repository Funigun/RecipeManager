using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Domain.Units.Enums;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Authorization;
using RecipeManager.Shared.Contracts.Units;

namespace RecipeManager.Api.Features.Units;

public static class CreateUnit
{
    public sealed record Request(string Name, string? ShortName, int Group, Guid? PrimaryUnit, int ConversionFactor = 1);

    public sealed record Response(UnitId Id);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IAppDbContext dbContext)
        {
            RuleFor(x => x.Name)
                .SetValidator(new UnitNameValidator())
                .MustAsync(async (name, cancellationToken) =>
                {
                    return !await dbContext.Units.AnyAsync(unit => unit.Name == name, cancellationToken);
                }).WithMessage("Unit Name must be unique");

            When(x => x.ShortName is not null, () =>
            {
                RuleFor(x => x.ShortName)
                    .SetValidator(new UnitShortNameValidator())
                    .MustAsync(async (shortName, cancellationToken) =>
                    {
                        return !await dbContext.Units.AnyAsync(unit => unit.ShortName == shortName, cancellationToken);
                    }).WithMessage("Unit Short Name must be unique");
            });

            RuleFor(x => x.Group)
                .Must(group => group.IsUnitGroup())
                    .WithMessage("Invalid unit group")
                    .WithName("Unit Group");

            When(x => x.PrimaryUnit is not null, () =>
            {
                RuleFor(x => x.PrimaryUnit)
                    .MustAsync(async (primaryUnitId, cancellationToken) =>
                    {
                        UnitId id = new(primaryUnitId!.Value);
                        return await dbContext.Units.AnyAsync(unit => unit.Id == id, cancellationToken);
                    }).WithMessage("Primary Unit must reference an existing unit");

                RuleFor(x => x.ConversionFactor)
                    .GreaterThan(1)
                    .WithMessage("Conversion Factor must be greater than 1 when Primary Unit is specified");
            });
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
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedPost<Request, Response>(string.Empty, Handler)
                     .WithName("CreateMeasurementUnit")
                     .WithDescription("Creates new measurement unit");
        }
    }

    public static async Task<IResult> Handler(Request request, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        Unit unit = request.ToUnit();

        dbContext.Units.Add(unit);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Created($"/api/units/{unit.Id}", unit.ToPostResponse());
    }

    private static Unit ToUnit(this Request request)
    {
        return Unit.Create(request.Name, request.ShortName, (UnitGroup)request.Group, request.PrimaryUnit, request.ConversionFactor);
    }

    private static Response ToPostResponse(this Unit unit)
    {
        return new Response(unit.Id);
    }
}
