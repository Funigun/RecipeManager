using FluentValidation;
using RecipeManager.UI.Blazor.Components.Common;

namespace RecipeManager.UI.Blazor.Features.Ingredients.CreateIngredient;

public sealed class IngredientForCreateValidator : BaseAbstractValidator<IngredientForCreateModel>
{
    public IngredientForCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Name is required.")
            .MaximumLength(100)
                .WithMessage("Name must not exceed 100 characters.");
    }
}
