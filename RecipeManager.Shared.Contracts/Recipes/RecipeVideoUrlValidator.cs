using FluentValidation;

namespace RecipeManager.Shared.Contracts.Recipes;

public class RecipeVideoUrlValidator : AbstractValidator<string?>
{
    public RecipeVideoUrlValidator()
    {
        RuleFor(x => x)
            .MaximumLength(2000);
    }
}
