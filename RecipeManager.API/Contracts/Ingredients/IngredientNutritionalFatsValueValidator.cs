using FluentValidation;

namespace RecipeManager.Api.Contracts.Ingredients;

public sealed class IngredientNutritionalFatsValueValidator : AbstractValidator<double>
{
    public IngredientNutritionalFatsValueValidator()
    {
        RuleFor(fats => fats)
            .GreaterThanOrEqualTo(0)
                .WithMessage("Fats value cannot be negative.");
    }
}
