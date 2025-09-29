using RecipeManager.Shared.Contracts.Ingredients;
using RecipeManager.UI.Blazor.Components.Common;

namespace RecipeManager.UI.Blazor.Features.Ingredients.CreateIngredient;

public sealed class IngredientForCreateValidator : BaseAbstractValidator<IngredientForCreateModel>
{
    public IngredientForCreateValidator()
    {
        RuleFor(x => x.Name).SetValidator(new IngredientNameValidator());
    }
}
