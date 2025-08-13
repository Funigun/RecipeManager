using FluentValidation;

namespace RecipeManager.Shared.Contracts.Recipes;

public class RecipeTitleValidator : AbstractValidator<string>
{
    public RecipeTitleValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .MaximumLength(100);
    }
}
