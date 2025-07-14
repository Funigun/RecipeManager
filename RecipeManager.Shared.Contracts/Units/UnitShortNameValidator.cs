using FluentValidation;

namespace RecipeManager.Shared.Contracts.Units;

public sealed class UnitShortNameValidator : AbstractValidator<string?>
{
    private const int MaxLength = 25;

    public UnitShortNameValidator()
    {
        When(shortName => shortName is not null, () =>
        {
            RuleFor(shortName => shortName)
                .NotEmpty()
                    .WithMessage("Short name cannot be empty")
                .MaximumLength(MaxLength)
                    .WithMessage($"Short name cannot exceed {MaxLength} characters");
        });
    }
}
