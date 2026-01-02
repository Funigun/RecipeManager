using FluentValidation;

namespace RecipeManager.Shared.Contracts.Ingredients;

public sealed class IngredientNutritionalProteinsValueValidator : AbstractValidator<double>
{
    public IngredientNutritionalProteinsValueValidator()
    {
        RuleFor(proteins => proteins)
            .GreaterThanOrEqualTo(0)
                .WithMessage("Proteins value cannot be negative.");
    }
}
