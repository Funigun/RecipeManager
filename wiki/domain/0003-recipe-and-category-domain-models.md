# 3. Recipe and RecipeCategory Domain Models explanation

Date: 2024-05-23
Status: Accepted

## Context

The `Recipe` and `RecipeCategory` domain models form the core content of the component system, capturing how users define culinary instructions and how those instructions are organized.

### Recipe
The `Recipe` model is a complex root aggregate that encapsulates all the details required to prepare a dish. It includes:
- **Core Metadata**: `Title`, `Description`, `ImageURL`, `VideoURL`.
- **Cooking Instructions**: Organized into `Sections` (e.g., "Preparation", "Cooking", "Plating"), each containing ordered `RecipeStep` entities.
- **Ingredients**: A collection of `RecipeIngredient` entities defining *what* and *how much* is used.
- **Properties**: `NumberOfServings`, `Difficulty` level, and `NutritionalValue`.
- **Output Definition**: Can optionally map to a produced `Ingredient` (via `IngredientId`), enabling recipes that create ingredients for other recipes (e.g., a "Pie Crust" recipe yielding a "Pie Crust" ingredient).
- **Yield**: `Amount` (quantity) and `Unit` specifying what the recipe produces.

### RecipeCategory
The `RecipeCategory` model provides a mechanism to classify recipes.
- **Properties**: `Name` and `Type`.
- **Type**: Unlike ingredients, recipe categories have a `Type` (enum) to distinguish between visual categories (e.g., "Breakfast", "Dinner") or other organizational structures.

For full feature details, see `Features/Recipes` and `Features/RecipeCategories`.

## Relationships

- **Recipe to RecipeCategory**:
    - A Many-to-Many relationship allows a recipe to appear in multiple categories (configured via `RecipeToRecipeCategory`).
- **Recipe to Ingredient (Output)**:
    - An optional One-to-One / Many-to-One relationship where a `Recipe` creates an `Ingredient` (e.g., Homemade Pasta). This allows nested recipe dependency graphs.
- **Recipe to RecipeIngredient**:
    - A One-to-Many relationship. `RecipeIngredient` links `Recipe`, `Ingredient`, and `Unit` to define quantities (e.g., 200g of Flour).
- **Recipe Sections**:
    - A One-to-Many owned relationship. `Recipe` owns `RecipeSection`, which in turn owns `RecipeStep`.
- **Recipe Amount**:
    - An owned entity `RecipeAmount` that links to a `Unit` to define the total yield of the recipe.

For specific database mappings, refer to:
- `Persistance/Configuration/Recipes/RecipeConfiguration.cs`
- `Persistance/Configuration/Recipes/RecipeCategoryConfiguration.cs`
- `Persistance/Configuration/Recipes/RecipeIngredientConfiguration.cs`
