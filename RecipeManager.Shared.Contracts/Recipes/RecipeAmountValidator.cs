using FluentValidation;

namespace RecipeManager.Shared.Contracts.Recipes;

public class RecipeAmountValidator : AbstractValidator<double>
{
    public RecipeAmountValidator()
    {
        RuleFor(x => x)
            .GreaterThan(0);
    }
}
