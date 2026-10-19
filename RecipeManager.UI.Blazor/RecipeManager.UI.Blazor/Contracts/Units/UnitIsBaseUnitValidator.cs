using FluentValidation;

namespace RecipeManager.UI.Blazor.Contracts.Units;

public class UnitIsBaseUnitValidator : AbstractValidator<bool>
{
    public UnitIsBaseUnitValidator()
    {
        // No additional rules, but placeholder for future logic or consistency
        RuleFor(x => x).NotNull().WithMessage("IsBaseUnit is required.");
    }
}
