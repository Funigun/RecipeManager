using FluentValidation;

namespace RecipeManager.Shared.Contracts.Recipes;

public class RecipeAmountValidator : AbstractValidator<double>
{
    public RecipeAmountValidator()
    {
        RuleFor(x => x)
            .GreaterThan(0)
            .WithMessage("Recipe amount must be greater than 0.");
    }
}
