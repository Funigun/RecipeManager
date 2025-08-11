using FluentValidation;

namespace RecipeManager.Shared.Contracts.Ingredients;

public sealed class IngredientNameValidator : AbstractValidator<string>
{
    private const int NameMaxLength = 100;

    public IngredientNameValidator()
    {
        RuleFor(name => name)
            .NotEmpty()
                .WithMessage("Ingredient name cannot be empty.")
            .MaximumLength(NameMaxLength)
                .WithMessage($"Ingredient name cannot exceed {NameMaxLength} characters.");
    }
}
