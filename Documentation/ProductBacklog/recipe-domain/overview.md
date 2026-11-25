---
status: approved
topic: Domain redesign
---
# Recipe Domain Redesign

## Summary
This redesign prepares the domain for shared and private ingredients, global catalogs, categorized day-based meal planning, and shopping lists whose quantities reflect how recipes were selected.

## Goals
- Make the minimum domain changes needed to prepare for the target state.
- Provide admin-managed global catalogs while allowing users to keep private ingredients.
- Support categorized meal plans and explain shopping-list totals based on available conversions.
- Use planned servings or saved recipe amounts as appropriate when generating shopping lists.

## Users and permissions
| Role | Can | Cannot |
|------|-----|--------|
| Regular user | <ul><li>Manage private ingredients, recipes, and Dishes.</li><li>Optionally map a private ingredient to a global ingredient.</li><li>Use global units, conversions, and categories.</li><li>Order global shopping-list categories for themselves.</li><li>Plan Dishes and generate shopping lists.</li></ul> | <ul><li>Create or maintain global ingredients, units, unit conversions, ingredient conversions, shopping-list categories, or meal categories.</li><li>Create custom units or shopping-list categories.</li><li>Manage recipe visibility, sharing, permissions, or copies as part of this redesign.</li></ul> |
| Admin | <ul><li>Do everything a regular user can.</li><li>Create and maintain global ingredients, units and unit conversions, global ingredient conversions, shopping-list categories, and meal categories.</li></ul> | <ul><li>No additional restriction was decided.</li></ul> |

## Model contexts
- [Ingredients](ingredients/prd.md)
- [Units](units/prd.md)
- [Categories](categories/prd.md)
- [Dishes](dishes/prd.md)
- [Recipes](recipes/prd.md)
- [Meal planning](meal-planning/prd.md)
- [Shopping lists](shopping-lists/prd.md)

## Scope and transition
- **R-01 — Domain scope** *(Source: D-01):* Make only the minimum domain changes needed to prepare for the target state. UI rebuilding and Azure learning are outside scope.
- **R-23 — Legacy data transition** *(Source: D-23):* Start with a clean database. Obsolete test data may be discarded; migration or manual recreation is not required.
- **AC-22:** The redesign starts with a clean database; obsolete test data may be discarded, and migration or manual recreation of that data is not required.

## Cross-cutting constraints
- Rebuilding the UI and Azure learning are outside this redesign's implementation scope.
- Additional screen layouts, loading and error states, and accessibility requirements were not specified.
- No performance, availability, retention, hosting, Azure, or other non-functional targets were specified.
- No changes to cookbook behavior were specified.

## Impact overview
- Ingredient, unit, category, Dish, recipe, meal-planning, and shopping-list changes are detailed in their respective context PRDs.
- The transition starts from a clean database; existing obsolete test data may be discarded without migration or manual recreation.

## Open questions
- No additional cross-context product questions are recorded here. See the relevant context PRD for model-specific questions.

## Documentation targets
- Update the relevant `wiki/domain` documents for Ingredients, Units, Recipes, categories, MealPlan, and shopping-list quantity rules.
- Create or update `wiki/adr` records for significant domain choices after deferred and open questions are resolved.
