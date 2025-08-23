using FluentValidation;

namespace RecipeManager.Shared.Contracts.Recipes;

public sealed class RecipeStepDescriptionValidator : AbstractValidator<string>
{
    public RecipeStepDescriptionValidator()
    {
        RuleFor(x => x)
            .MaximumLength(1000);
    }
}
