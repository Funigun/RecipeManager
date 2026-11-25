---
status: approved
topic: Domain redesign — Recipes
source: decisions.md
---
# Recipes

## Summary
Keep recipe serving counts and nutrition on recipes. Recipe visibility, sharing, permissions, and independent copies remain deferred.

## Users and permissions
See the shared roles and permissions in the [domain redesign overview](../overview.md#users-and-permissions).

## Domain rules

### Recipe scope and ownership

- **R-13 — Recipe visibility** *(Entities: Recipe; Source: D-13)*
  - Recipe visibility is deferred; visibility levels are not part of this redesign's target.
- **R-14 — Recipe sharing and copies** *(Entities: Recipe; Source: D-14)*
  - Recipe sharing, friend permissions, and independent recipe copies are deferred and are not part of this redesign's target.
- **R-27 — Recipe serving count** *(Entities: Recipe, meal-plan entry, shopping list; Source: D-27)*
  - A recipe's saved ingredient amounts make its stated number of servings.
  - Plan-based quantities scale by planned servings divided by the recipe's saved serving count. See [Shopping lists](../shopping-lists/prd.md#user-stories).
- **R-34 — Nutrition ownership** *(Entities: Ingredient, recipe, nutrition; Source: D-34)*
  - Nutrition belongs to recipes; ingredients do not have ingredient-level nutrition.

## Product constraints (not user stories)
- **AC-13 (deferred):** This redesign does not add recipe visibility levels.
- **AC-14 (deferred):** This redesign does not add friend sharing or sharing-permission behavior.
- **AC-15 (deferred):** This redesign does not add an independent-copy capability for another user's recipe.
- **AC-31:** Nutrition belongs to recipes and is not stored on ingredients.

**Related domain rules:** R-13, R-14, R-27, R-34

## Impact on existing system
The current `Recipe` model includes a serving count, ingredient list, nutrition, sections, steps, and an optional link from an ingredient to its preparation recipe. Recipe visibility, sharing, permissions, and copies are explicitly deferred.

## Documentation targets
- Update the Recipes explanation in `wiki/domain`.
- Create or update `wiki/adr` records for significant recipe-domain choices after deferred and open questions are resolved.
