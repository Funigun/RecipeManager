using FluentValidation;

namespace RecipeManager.Api.Contracts.Recipes;

public sealed class RecipeStepDescriptionValidator : AbstractValidator<string>
{
    public RecipeStepDescriptionValidator()
    {
        RuleFor(x => x)
            .MaximumLength(1000);
    }
}
