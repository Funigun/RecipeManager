using FluentValidation;

namespace RecipeManager.UI.Blazor.Contracts.Ingredients;

public sealed class IngredientNutritionalAmountValidator : AbstractValidator<int>
{
    public IngredientNutritionalAmountValidator()
    {
        RuleFor(ingredientNutritionalAmount => ingredientNutritionalAmount)
            .GreaterThanOrEqualTo(0)
                .WithMessage(errorMessage: "Amount for which nutitional values are provided cannot be negative.");
    }
}
