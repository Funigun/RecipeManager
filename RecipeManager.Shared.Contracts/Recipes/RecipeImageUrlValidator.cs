using FluentValidation;

namespace RecipeManager.Shared.Contracts.Recipes;

public class RecipeImageUrlValidator : AbstractValidator<string?>
{
    public RecipeImageUrlValidator()
    {
        RuleFor(x => x)
            .MaximumLength(2000);
    }
}
