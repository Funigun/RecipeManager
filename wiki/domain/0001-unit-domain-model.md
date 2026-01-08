# 1. Unit Domain Model explanation

Date: 2024-05-23
Status: Accepted

## Context

The `Unit` domain model is central to the RecipeManager application, responsible for defining measurement standards used across recipes and ingredients. It addresses the requirement to support various measurement systems (metric, imperial) and custom units (e.g., "cup", "pinch").

Key capabilities include:
- **Identification**: Unique names and abbreviations (ShortName) for display.
- **Pluralization**: Support for plural forms (PluralName, PluralShortName) for correct UI formatting.
- **Conversion Logic**: The model supports unit conversions through a `PrimaryUnit` reference and a `ConversionFactor`. A unit can be a Base Unit (e.g., Gram) or a derived unit (e.g., Kilogram = 1000 Grams).
- **Categorization**: Units are grouped via `UnitGroup` (e.g., Weight, Volume, Item).

This model must ensure data integrity, such as preventing circular dependencies in conversions (enforced by business logic) and ensuring required fields like Name and PluralName are present.

For more details on the operations available for this model, you can check the features under the `Features/Units` folder.

## Relationships

The `Unit` model is highly interconnected within the domain, acting as a foundational entity for quantity definitions.

- **Self-Reference**: A `Unit` can reference another `Unit` via the `PrimaryUnit` property. This is configured in `Persistance/Configuration/Units/UnitsConfiguration.cs` with a `Restrict` delete behavior to prevent cascading deletes of primary units.
- **Ingredient**: 
    - Referenced by `Ingredient.BaseUnit` to define the standard measurement for an ingredient.
    - Used in `IngredientPackage` (PackageUnitId, PackageSizeUnitId) to define purchasing units.
- **IngredientUnitConvertion**: Used to define specific conversion rates for ingredients (e.g., 1 Cup of Flour = 120 Grams).
- **RecipeIngredient** (implied): Although not explicitly seen in the `Unit` class, the `Recipe` domain uses `Unit` to specify the amount of an ingredient used in a dish.

For specific database configuration and relationship constraints, you can check `Persistance/Configuration/Units/UnitsConfiguration.cs`.
