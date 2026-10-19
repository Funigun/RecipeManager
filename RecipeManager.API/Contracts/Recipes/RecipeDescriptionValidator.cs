using FluentValidation;

namespace RecipeManager.Api.Contracts.Recipes;

public class RecipeDescriptionValidator : AbstractValidator<string>
{
    public RecipeDescriptionValidator()
    {
        RuleFor(x => x)
            .MaximumLength(500);
    }
}
