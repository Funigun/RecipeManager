using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Domain.Units.Enums;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Authorization;
using RecipeManager.Shared.Contracts.Units;

namespace RecipeManager.Api.Features.Units;

public static class UpdateUnit
{
    public sealed record Request(
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
        public Validator(IUnitOfWork unitOfWork, IHttpContextAccessor context)
        {
            RuleFor(x => x.Name)
                .SetValidator(new UnitNameValidator())
                .MustAsync(async (name, cancellationToken) =>
                {
                    string id = context.HttpContext!.GetRouteData().Values["unitId"]!.ToString()!;
                    UnitId unitId = new(Guid.Parse(id));
                    return !await unitOfWork.Units.AnyByNameAsync(name, unitId, cancellationToken);
                }).WithMessage("Unit Name must be unique");

            When(x => x.ShortName is not null, () =>
            {
                RuleFor(x => x.ShortName)
                    .SetValidator(new UnitShortNameValidator())
                    .MustAsync(async (shortName, cancellationToken) =>
                    {
                        string id = context.HttpContext!.GetRouteData().Values["unitId"]!.ToString()!;
                        UnitId unitId = new(Guid.Parse(id));
                        return !await unitOfWork.Units.AnyByShortNameAsync(shortName, unitId, cancellationToken);
                    }).WithMessage("Unit Short Name must be unique");
            });

            RuleFor(x => x.PluralName)
                .SetValidator(new UnitPluralNameValidator());

            When(x => x.PluralShortName is not null, () =>
            {
                RuleFor(x => x.PluralShortName)
                    .SetValidator(new UnitPluralShortNameValidator());
            });

            RuleFor(x => x.IsBaseUnit)
                .SetValidator(new UnitIsBaseUnitValidator());

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
                        return await unitOfWork.Units.ExistsAsync(unit => unit.Id == id, cancellationToken);
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
    public class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedPut<Request, Request>("/{unitId}", Handler)
                     .WithName("UpdateMeasurementUnit")
                     .WithDescription("Updates a measurement unit");
        }
    }

    internal static async Task<Results<NoContent, NotFound, BadRequest>> Handler(Guid unitId, [FromBody] Request request, [FromServices] IUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        UnitId id = new(unitId);

        Unit? unit = await unitOfWork.Units.GetByIdAsync(id, cancellationToken)
                  ?? throw new EntityNotFoundException<Unit, UnitId>(id);

        unit.Update(
            request.Name,
            request.ShortName,
            request.PluralName,
            request.PluralShortName,
            (UnitGroup)request.Group,
            request.IsBaseUnit,
            request.PrimaryUnit,
            request.ConversionFactor
        );

        await unitOfWork.Units.Update(unit, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
