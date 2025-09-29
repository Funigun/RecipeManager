using RecipeManager.Shared.Contracts.Ingredients;
using RecipeManager.UI.Blazor.Components.Common;

namespace RecipeManager.UI.Blazor.Features.Ingredients.UpdateIngredient;

public sealed class IngredientForUpdateValidator : BaseAbstractValidator<IngredientForUpdateModel>
{
    public IngredientForUpdateValidator()
    {
        RuleFor(x => x.Name).SetValidator(new IngredientNameValidator());
    }
}
