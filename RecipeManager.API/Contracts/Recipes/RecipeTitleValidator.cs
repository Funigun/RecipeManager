using FluentValidation;

namespace RecipeManager.Api.Contracts.Recipes;

public class RecipeTitleValidator : AbstractValidator<string>
{
    public RecipeTitleValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .MaximumLength(100);
    }
}
