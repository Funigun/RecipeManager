namespace RecipeManager.Shared.Contracts.Authorization;

public enum UserPermission
{
    ReadUnit,
    WriteUnit,
    ReadRecipeCategory,
    WriteRecipeCategory,
    ReadIngredientCategory,
    WriteIngredientCategory,
    ReadRecipe,
    WriteRecipe,
    ReadCookbook,
    WriteCookbook,
}