using FluentValidation;

namespace RecipeManager.Shared.Contracts.Ingredients;

public sealed class IngredientNutritionalCarbohydratesValueValidator : AbstractValidator<double>
{
    public IngredientNutritionalCarbohydratesValueValidator()
    {
        RuleFor(carbohydrates => carbohydrates)
            .GreaterThanOrEqualTo(0)
                .WithMessage("Carbohydrates value cannot be negative.");
    }
}
