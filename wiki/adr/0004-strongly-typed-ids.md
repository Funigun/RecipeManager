# 4. Strongly Typed IDs with EF Core

Date: 2024-09-01
Status: Accepted

## Context
The domain contains many entities (Recipes, Ingredients, Units, Categories) that use `Guid` as primary keys. Passing raw `Guid`s around led to "primitive obsession" issues, where a `UnitId` could accidentally be passed to a method expecting a `RecipeId`, causing subtle bugs only detectable at runtime (or silent failures).

## Decision
We decided to use **Strongly Typed IDs** for all domain entities.

- IDs are implemented as specific types (structs/records) wrapping a `Guid` (e.g., `RecipeId`, `IngredientId`).
- Entity Framework Core is configured with **Value Converters** (e.g., `RecipeIdConverter`) to store these as native GUIDs in the database but expose them as types in code.

## Consequences
### Positive
- **Type Safety**: The compiler prevents accidental swapping of IDs.
- **Readability**: Method signatures like `GetRecipe(RecipeId id)` are self-documenting.

### Negative
- **Boilerplate**: Requires creating a Type and a specific EF Converter for every entity.
- **Serialization**: JSON serialization requires custom converters to treat these types as simple strings/guids in API responses (handled in API configurations).
