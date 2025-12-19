using FluentValidation;

namespace RecipeManager.Shared.Contracts.Units;

public class UnitPluralShortNameValidator : AbstractValidator<string?>
{
    public UnitPluralShortNameValidator()
    {
        RuleFor(x => x)
            .MaximumLength(30)
                .WithMessage("Plural short name must be at most 20 characters.");
    }
}
