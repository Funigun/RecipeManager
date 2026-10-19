using FluentValidation;

namespace RecipeManager.UI.Blazor.Contracts.Recipes;

public class RecipeTitleValidator : AbstractValidator<string>
{
    public RecipeTitleValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .MaximumLength(100);
    }
}
