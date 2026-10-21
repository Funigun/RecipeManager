using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Domain.Units.Enums;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Units;

public static class CreateUnit
{
    public sealed record Request(
        string Name,
        string? ShortName,
        string PluralName,
        string? PluralShortName,
        int Group,
        bool IsBaseUnit,
        Guid? PrimaryUnit,
        int ConversionFactor = 1
    );

    public sealed record Response(
        UnitId Id,
        string Name,
        string? ShortName,
        string PluralName,
        string? PluralShortName,
        int Group,
        bool IsBaseUnit,
        Guid? PrimaryUnit,
        int ConversionFactor
    );

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Name)
                .ValidateUnitName()
                .MustHaveUniqueUnitName(unitOfWork, "Unit Name must be unique");

            When(x => x.ShortName is not null, () =>
            {
                RuleFor(x => x.ShortName)
                    .ValidateUnitShortName()
                    .MustHaveUniqueUnitShortName(unitOfWork, "Unit Short Name must be unique");
            });

            RuleFor(x => x.PluralName)
                .ValidateUnitPluralName();

            When(x => x.PluralShortName is not null, () =>
            {
                RuleFor(x => x.PluralShortName)
                    .ValidateUnitPluralShortName();
            });

            RuleFor(x => x.IsBaseUnit)
                .ValidateUnitIsBaseUnit();

            RuleFor(x => x.Group)
                .Must(group => group.IsUnitGroup())
                    .WithMessage("Invalid unit group")
                    .WithName("Unit Group");

            When(x => x.PrimaryUnit is not null, () =>
            {
                RuleFor(x => x.PrimaryUnit)
                    .MustReferenceExistingUnit(unitOfWork, "Primary Unit must reference an existing unit");

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

    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedPost<Request, Response>(UnitsGroup.Route, Handler)
                     .WithName("CreateMeasurementUnit")
                     .WithDescription("Creates new measurement unit")
                     .WithTags(UnitsGroup.Tag)
                     .RequireAuthorization("RecipeManagerPolicy")
                     .ProducesProblem(StatusCodes.Status401Unauthorized);
        }
    }

    public static async Task<IResult> Handler(Request request, [FromServices] IUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        Unit unit = request.ToUnit();

        await unitOfWork.Units.Add(unit, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Created(UnitsGroup.UnitById(unit.Id.Value), unit.ToPostResponse());
    }

    private static Unit ToUnit(this Request request)
    {
        return Unit.Create(
            request.Name,
            request.ShortName,
            request.PluralName,
            request.PluralShortName,
            (UnitGroup)request.Group,
            request.IsBaseUnit,
            request.PrimaryUnit,
            request.ConversionFactor
        );
    }

    private static Response ToPostResponse(this Unit unit)
    {
        return new Response(
            unit.Id,
            unit.Name,
            unit.ShortName,
            unit.PluralName,
            unit.PluralShortName,
            (int)unit.Group,
            unit.IsBaseUnit,
            unit.PrimaryUnit?.Value,
            unit.ConversionFactor
        );
    }
}
