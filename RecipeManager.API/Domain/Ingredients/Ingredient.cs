using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Common.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Domain.Ingredients;

public sealed class Ingredient : AuditableEntity, IEntity<IngredientId>
{
    private List<RecipeId> _recipes = [];
    private List<IngredientCategoryId> _categories = [];
    private List<IngredientUnitConvertion> _ingredientUnitConvertions = [];

    public IngredientId Id { get; set; } = default!;

    public string Name { get; set; } = string.Empty;

    public NutritionalValue NutritionalValue { get; set; } = new();

    public UnitId? BaseUnit { get; set; }

    public IngredientCategoryId? ShoppingListCategoryId { get; set; }

    public IngredientPackage IngredientPackage { get; set; } = default!;

    public IReadOnlyList<IngredientCategoryId> Categories => _categories.ToList();

    public IReadOnlyList<RecipeId> Recipes => _recipes.ToList();

    public IReadOnlyList<IngredientUnitConvertion> IngredientUnitConvertions => _ingredientUnitConvertions.ToList();

    private Ingredient()
    {
    }

    public static Ingredient Create(string name, NutritionalValue nutritionalValue, UnitId? baseUnit, IngredientPackage ingredientPackage, IEnumerable<IngredientUnitConvertion> ingredientUnitConvertions, IngredientCategoryId? ingredientCategoryId, IEnumerable<IngredientCategoryId> categoryIds, IEnumerable<RecipeId> recipeIds)
    {
        return new()
        {
            Name = name,
            ShoppingListCategoryId = ingredientCategoryId,
            NutritionalValue = nutritionalValue,
            BaseUnit = baseUnit,
            IngredientPackage = ingredientPackage,
            _ingredientUnitConvertions = ingredientUnitConvertions.ToList(),
            _categories = categoryIds.ToList(),
            _recipes = recipeIds.ToList()
        };
    }

    public void Update(string name, NutritionalValue nutritionalValue, UnitId? baseUnit, IngredientPackage ingredientPackage, IEnumerable<IngredientUnitConvertion> ingredientUnitConvertions, IngredientCategoryId? ingredientCategoryId, IEnumerable<IngredientCategoryId> categoryIds, IEnumerable<RecipeId> recipeIds)
    {
        Name = name;
        ShoppingListCategoryId = ingredientCategoryId;
        NutritionalValue.Calories = nutritionalValue.Calories;
        NutritionalValue.Proteins = nutritionalValue.Proteins;
        NutritionalValue.Fats = nutritionalValue.Fats;
        NutritionalValue.Carbohydrates = nutritionalValue.Carbohydrates;
        NutritionalValue.IngredientAmount = nutritionalValue.IngredientAmount;
        NutritionalValue.IngredientUnit = nutritionalValue.IngredientUnit;
        BaseUnit = baseUnit;
        IngredientPackage = ingredientPackage;
        _ingredientUnitConvertions = ingredientUnitConvertions.ToList();
        _categories = categoryIds.ToList();
        _recipes = recipeIds.ToList();
    }

    public IngredientCategoryId GetShoppingListGroupId()
    {
        return ShoppingListCategoryId is not null
                                      ? ShoppingListCategoryId! 
                                      : _categories.FirstOrDefault() ?? Guid.Empty;
    }
}
