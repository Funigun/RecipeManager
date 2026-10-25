using FluentValidation;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Domain.Units.Enums;
using RecipeManager.Api.Features.Validation;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Units.Command.UpdateUnit;

public sealed class UpdateUnitAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<UpdateUnitRequest>
{
    public Task<bool> IsAuthorized(UpdateUnitRequest request)
    {
        return Task.FromResult(currentUser.HasRole(UserRoles.Admin));
    }
}

public sealed class UpdateUnitValidationPolicy : BaseValidator<UpdateUnitRequest>
{
    public UpdateUnitValidationPolicy(IAppDbContext dbContext, IHttpContextAccessor context)
    {
        UnitId GetExcludedId()
        {
            string routeUnitId = context.HttpContext!.GetRouteData().Values["unitId"]!.ToString()!;
            return new UnitId(Guid.Parse(routeUnitId));
        }

        RuleFor(x => x.Name)
            .ValidateUnitName()
            .MustHaveUniqueUnitName(dbContext, GetExcludedId, "Unit Name must be unique");

        When(x => x.ShortName is not null, () =>
        {
            RuleFor(x => x.ShortName)
                .ValidateUnitShortName()
                .MustHaveUniqueUnitShortName(dbContext, GetExcludedId, "Unit Short Name must be unique");
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
                .MustReferenceExistingUnit(dbContext, "Primary Unit must reference an existing unit");

            RuleFor(x => x.ConversionFactor)
                .GreaterThan(1)
                .WithMessage("Conversion Factor must be greater than 1 when Primary Unit is specified");
        });
    }
}
