using FluentValidation;

namespace RecipeManager.Shared.Contracts.Ingredients;

public sealed class IngredientNutritionalCaloriesValueValidator : AbstractValidator<int>
{
    public IngredientNutritionalCaloriesValueValidator()
    {
        RuleFor(calories => calories)
            .GreaterThanOrEqualTo(0)
                .WithMessage("Calories value cannot be negative.");
    }
}
