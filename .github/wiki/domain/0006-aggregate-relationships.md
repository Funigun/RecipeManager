# Aggregate relationships

This page summarizes the aggregate boundaries and cross-aggregate references in the RecipeManager domain. Solid arrows indicate configured relational relationships; dashed arrows indicate IDs held as values without a configured FK to the target aggregate. Composition arrows show content managed with its owning root.

```mermaid
flowchart LR
    Unit[Unit]
    Ingredient[Ingredient]
    IngredientCategory[IngredientCategory]
    Package[IngredientPackage]
    Conversion[IngredientUnitConvertion]
    Recipe[Recipe]
    RecipeCategory[RecipeCategory]
    RecipeIngredient[RecipeIngredient]
    RecipeAmount[RecipeAmount]
    Sections[RecipeSection and RecipeStep]
    Cookbook[Cookbook]
    CookbookCategory[CookbookCategory]
    MealPlan[MealPlan]

    Unit -->|PrimaryUnit self-FK; Restrict| Unit
    Ingredient -->|optional BaseUnit FK| Unit
    Package -->|unit FKs| Unit
    Conversion -->|unit FK| Unit
    Ingredient -->|1:1 dependent; cascade| Package
    Ingredient -->|1:many dependent; cascade| Conversion
    Ingredient -->|optional ShoppingListCategoryId FK| IngredientCategory
    Ingredient -.->|Categories IDs; no category FK| IngredientCategory
    Ingredient -.->|Recipes IDs; no recipe FK| Recipe

    Recipe -->|owns| RecipeAmount
    Recipe -->|owns| Sections
    Recipe -->|contains lines| RecipeIngredient
    RecipeIngredient -->|IngredientId FK; ClientCascade| Ingredient
    RecipeIngredient -->|UnitId FK; Restrict| Unit
    RecipeAmount -->|UnitId; optional configured FK| Unit
    Recipe -->|optional IngredientId FK; SetNull| Ingredient
    Recipe -.->|Category IDs; no category FK| RecipeCategory

    Cookbook -->|contains categories| CookbookCategory
    CookbookCategory -->|ParentId self-reference; NoAction| CookbookCategory
    CookbookCategory -.->|Recipe IDs; no recipe FK| Recipe
    MealPlan -.->|Recipe IDs; no recipe FK| Recipe
```

## Boundary summary

| Aggregate root | Content managed with the root | References to other aggregates |
| --- | --- | --- |
| `Unit` | Unit data and its optional primary-unit reference | Referenced by ingredient, recipe, and package/conversion data |
| `Ingredient` | Nutritional JSON component, package, unit conversions, and stored ID collections | Optional base unit; optional shopping-list category; recipe IDs |
| `IngredientCategory` | Category name | Referenced by ingredient IDs |
| `Recipe` | Yield amount, sections/steps, recipe-ingredient lines, and category ID collection | Ingredients, units, output ingredient, and recipe categories |
| `RecipeCategory` | Category name and type | Referenced by recipe IDs |
| `Cookbook` | Cookbook categories and their subcategory tree | Recipe IDs stored on cookbook categories |
| `MealPlan` | Date and recipe ID collection | Recipe IDs |

## Relationship notes

- Recipe and ingredient-category membership, cookbook recipe placement, meal-plan recipe selection, and `Ingredient.Recipes` are ID collections stored in owned tables. Their target IDs are not configured as FKs to the target aggregate tables.
- `RecipeIngredient` is different: it is a dependent row owned by a recipe, with configured foreign keys to its ingredient and unit. The recipe output `IngredientId` is also a configured FK, separate from the ID collection on `Ingredient`.
- `IngredientCategory` has two distinct paths from `Ingredient`: the general `Categories` ID collection and the optional `ShoppingListCategoryId` FK used for shopping-list grouping.
- `CreatedBy` on auditable roots is the user ownership marker used by user-scoped features; it is a string audit value, not a relationship to an identity aggregate.

## Source

- `RecipeManager.API/Domain/`
- `RecipeManager.API/Persistance/AppDbContext.cs`
- `RecipeManager.API/Persistance/Configuration/`
- `RecipeManager.API/Features/Cookbooks/GetCookbooks.cs`
- `RecipeManager.API/Features/MealPlanner/GetMealPlan.cs`
