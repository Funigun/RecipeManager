using FluentValidation;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Features.Validation;

namespace RecipeManager.Api.Features.Ingredients.Command.CreateIngredient;

public sealed class CreateIngredientValidationPolicy : BaseValidator<CreateIngredientRequest>
{
    public CreateIngredientValidationPolicy(IAppDbContext dbContext)
    {
        RuleFor(request => request.Name).ValidateIngredientName();

        RuleFor(request => request.NutritionalValues.Calories).ValidateNutritionalCalories();
        RuleFor(request => request.NutritionalValues.Carbohydrates).ValidateNutritionalCarbohydrates();
        RuleFor(request => request.NutritionalValues.Fats).ValidateNutritionalFats();
        RuleFor(request => request.NutritionalValues.Proteins).ValidateNutritionalProteins();
        RuleFor(request => request.NutritionalValues.IngredientAmount).ValidateNutritionalAmount();

        RuleFor(request => request.NutritionalValues.IngredientUnitId)
            .MustReferenceExistingUnit(dbContext, "Ingredient unit does not exist");

        RuleFor(request => request.BaseUnit)
            .MustReferenceExistingUnit(dbContext, "Base unit does not exist");

        RuleFor(request => request.IngredientPackage.PackageUnitId)
            .MustReferenceExistingUnit(dbContext, "Package unit does not exist");

        RuleFor(request => request.IngredientPackage.PackageSize)
            .GreaterThan(0).WithMessage("Package size must be greater than 0");

        RuleFor(request => request.IngredientPackage.PackageSizeUnitId)
            .MustReferenceExistingUnit(dbContext, "Package size unit does not exist");

        RuleForEach(request => request.IngredientUnitConvertions).ChildRules(conversion =>
        {
            conversion.RuleFor(item => item.UnitToConvertId)
                .MustReferenceExistingUnit(dbContext, "Unit to convert does not exist");
            conversion.RuleFor(item => item.Ratio).GreaterThan(0).WithMessage("Ratio must be greater than 0");
        });

        When(request => request.ShoppingListCategoryId.HasValue, () =>
        {
            RuleFor(request => request.ShoppingListCategoryId)
                .MustReferenceExistingIngredientCategory(dbContext, "Shopping list category does not exist")
                .Must((request, categoryId) => !request.Categories.Contains(categoryId!.Value))
                .WithMessage("Categories can not include the ShoppingListCategoryId.");
        });

        When(request => request.Categories.Any(), () =>
        {
            RuleFor(request => request.Categories)
                .MustHaveUniqueIds(categoryId => categoryId, "Duplicate categories are not allowed.")
                .MustReferenceExistingIngredientCategories(
                    dbContext,
                    categories => categories,
                    "Some of categories does not exist");
        });

        When(request => request.Recipes.Any(), () =>
        {
            RuleFor(request => request.Recipes)
                .MustHaveUniqueIds(recipeId => recipeId, "Duplicate recipes are not allowed.")
                .MustReferenceExistingRecipes(dbContext, recipes => recipes, "Some of recipes does not exist");
        });
    }
}
