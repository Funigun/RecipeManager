using FluentValidation;

namespace RecipeManager.Api.Contracts.Recipes;

public class RecipeVideoUrlValidator : AbstractValidator<string?>
{
    public RecipeVideoUrlValidator()
    {
        RuleFor(x => x)
            .MaximumLength(2000);
    }
}
