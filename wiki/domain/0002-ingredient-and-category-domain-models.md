# 2. Ingredient and IngredientCategory Domain Models explanation

Date: 2024-05-23
Status: Accepted

## Context

The `Ingredient` and `IngredientCategory` domain models manage the raw materials used in recipes and their classification within the RecipeManager application.

### Ingredient
The `Ingredient` model represents a specific food item or component used in cooking. It is a rich entity that encapsulates:
- **Identification**: Name of the ingredient.
- **Nutritional Information**: Detailed nutritional values (Calories, Proteins, Fats, Carbohydrates) stored as a complex value object `NutritionalValue`.
- **Measurement Standards**: 
    - `BaseUnit`: The standard unit for the ingredient (e.g., Grams for flour).
    - `IngredientUnitConvertion`: Specific conversion rates (e.g., converting "Cup" to "Grams" for this specific ingredient).
- **Purchasing Details**: Encapsulated in `IngredientPackage` (e.g., sold in 1kg blocks).
- **Categorization**: Can belong to multiple categories (e.g., "Dairy", "Refrigerated") for organization, and a specific `ShoppingListCategoryId` for grouping in shopping lists.

### IngredientCategory
The `IngredientCategory` model is a simple classification entity used to tag and group ingredients. It allows users to browse ingredients by type (e.g., "Vegetables", "Spices") and organize their shopping lists efficiently.

For more details on operations, see `Features/Ingredients` and `Features/IngredientCategories`.

## Relationships

These models are central to the recipe and shopping logic:

- **Ingredient to IngredientCategory**:
    - **General Categorization**: A Many-to-Many relationship allows an ingredient to belong to multiple categories (configured via `IngredientToIngredientCategory`).
    - **Shopping List Grouping**: A One-to-Many optional relationship via `ShoppingListCategoryId` designates a specific category for grouping items on a generated shopping list.
- **Ingredient to Unit**:
    - Uses `Unit` for `BaseUnit` to define its fundamental measurement.
    - Uses `Unit` within `IngredientPackage` to define package sizes.
    - Uses `Unit` in `IngredientUnitConvertion` to map alternative units back to the base unit.
- **Ingredient to Recipe**: A Many-to-Many relationship tracks recipes to create specific ingredient e.g. we have recipe for spaghetti noodles (flour + eggs) and recipe for spaghetti which contains spaghetti noodles as ingredient.
- **Ingredient to IngredientPackage**: A One-to-One relationship stores purchasing specifics for the ingredient.

For specific database mappings, refer to:
- `Persistance/Configuration/Ingredients/IngredientConfiguration.cs`
- `Persistance/Configuration/Ingredients/IngredientCategoryConfiguration.cs`
