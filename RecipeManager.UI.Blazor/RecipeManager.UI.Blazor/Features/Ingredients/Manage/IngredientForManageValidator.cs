using RecipeManager.Shared.Contracts.Ingredients;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Components.Common;

namespace RecipeManager.UI.Blazor.Features.Ingredients.Manage;

public sealed class IngredientForManageValidator : BaseAbstractValidator<HateoasResponse<IngredientForManageModel>>
{
    public IngredientForManageValidator()
    {
        RuleFor(x => x.Item.Name).SetValidator(new IngredientNameValidator());

        RuleFor(x => x.Item.NutritionalValues.Calories).SetValidator(new IngredientNutritionalCaloriesValueValidator());
        RuleFor(x => x.Item.NutritionalValues.Carbohydrates).SetValidator(new IngredientNutritionalCarbohydratesValueValidator());
        RuleFor(x => x.Item.NutritionalValues.Fats).SetValidator(new IngredientNutritionalFatsValueValidator());
        RuleFor(x => x.Item.NutritionalValues.Proteins).SetValidator(new IngredientNutritionalProteinsValueValidator());
        RuleFor(x => x.Item.NutritionalValues.IngredientAmount).SetValidator(new IngredientNutritionalAmountValidator());
    }
}
