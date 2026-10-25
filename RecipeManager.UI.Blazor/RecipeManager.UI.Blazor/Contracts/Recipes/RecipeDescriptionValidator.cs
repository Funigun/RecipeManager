using FluentValidation;

namespace RecipeManager.UI.Blazor.Contracts.Recipes;

public class RecipeDescriptionValidator : AbstractValidator<string>
{
    public RecipeDescriptionValidator()
    {
        RuleFor(x => x)
            .MaximumLength(500);
    }
}
