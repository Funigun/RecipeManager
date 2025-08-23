using FluentValidation;

namespace RecipeManager.Shared.Contracts.Recipes;

public class RecipeDescriptionValidator : AbstractValidator<string>
{
    public RecipeDescriptionValidator()
    {
        RuleFor(x => x)
            .MaximumLength(500);
    }
}
