using FluentValidation;

namespace RecipeManager.Shared.Contracts.Units;

public class UnitPluralNameValidator : AbstractValidator<string>
{
    public UnitPluralNameValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
                .WithMessage("Plural name is required.")
            .MaximumLength(55)
                .WithMessage("Plural name must be at most 100 characters.");
    }
}
