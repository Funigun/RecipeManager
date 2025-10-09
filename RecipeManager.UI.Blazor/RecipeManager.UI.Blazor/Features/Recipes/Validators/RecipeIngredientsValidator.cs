using FluentValidation;
using RecipeManager.Shared.Contracts.Recipes;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.Recipes.Models;

namespace RecipeManager.UI.Blazor.Features.Recipes.Validators;

public sealed class RecipeIngredientsValidator : BaseAbstractValidator<IEnumerable<RecipeIngredientModel>>
{
    public RecipeIngredientsValidator()
    {
        RuleFor(ingredients => ingredients)
            .NotEmpty()
            .WithMessage("Recipe requires at leas one ingredient");
    }
}
