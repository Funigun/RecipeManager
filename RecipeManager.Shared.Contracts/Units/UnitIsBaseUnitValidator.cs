using FluentValidation;

namespace RecipeManager.Shared.Contracts.Units;

public class UnitIsBaseUnitValidator : AbstractValidator<bool>
{
    public UnitIsBaseUnitValidator()
    {
        // No additional rules, but placeholder for future logic or consistency
        RuleFor(x => x).NotNull().WithMessage("IsBaseUnit is required.");
    }
}
