using FluentValidation;

namespace RecipeManager.Shared.Contracts.Units;

public sealed class UnitNameValidator : AbstractValidator<string>
{
    private const int MaxLength = 50;

    public UnitNameValidator()
    {
        RuleFor(name => name)
            .NotEmpty()
                .WithMessage("Unit name cannot be empty")
            .MaximumLength(MaxLength)
                .WithMessage($"Unit name cannot exceed {MaxLength} characters");
    }
}
