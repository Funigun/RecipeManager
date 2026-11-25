---
status: approved
topic: Domain redesign — Dishes
source: decisions.md
---
# Dishes

## Summary
Provide user-owned Dishes as meal-planning items that can stand alone or have alternative preparation Recipes. Use the nutrition for the planned Dish or selected preparation when calculating daily nutrition.

## Users and permissions
See the shared roles and permissions in the [domain redesign overview](../overview.md#users-and-permissions).

## Domain rules

### Dish ownership and preparation

- **R-39 — Dish ownership** *(Entities: User, Dish; Source: D-11)*
  - Each user creates and manages their own Dishes.
- **R-40 — Alternative preparation Recipes** *(Entities: Dish, Recipe; Source: D-10, D-12)*
  - A Dish may have zero or more associated Recipes.
  - Associated Recipes are alternative preparations, not components combined into the Dish.

### Dish nutrition and meal planning

- **R-41 — Standalone Dish nutrition** *(Entities: Dish, nutrition; Source: D-13)*
  - A Dish without associated Recipes must have nutrition values for calories, protein, fat, and carbohydrates.
- **R-42 — Per-serving Dish nutrition** *(Entities: Dish, meal-plan entry; Source: D-13)*
  - Dish nutrition describes one serving, and each Meal-Plan Entry for a Dish represents one serving.
- **R-43 — Nutrition for a selected preparation** *(Entities: Dish, Recipe, meal-plan entry; Source: D-14)*
  - When a planned Dish has associated Recipes, the user selects the Recipe being prepared and its nutrition is used for the day's calculation.
- **R-44 — Nutrition for a standalone Dish** *(Entities: Dish, meal-plan entry; Source: D-14)*
  - When a planned Dish has no associated Recipes, its manually specified nutrition is used for the day's calculation.
- **R-46 — Dish identity and image** *(Entities: Dish; Source: D-17)*
  - Every Dish has a required name and image URL.
  - Name length and image URL constraints are not specified.
- **R-47 — Recipe main image** *(Entities: Recipe, Dish; Source: D-18)*
  - Recipe.ImageUrl remains optional and separate from the required Dish image. Recipe-step images remain supported.
- **R-48 — Missing nutrition in daily totals** *(Entities: meal-plan entry, nutrition total; Source: D-19)*
  - If nutrition is unavailable for a planned item, mark the daily nutrition total incomplete rather than treating missing values as zero.

## User stories

### US-18 — Manage personal Dishes
As a user, I want to create and manage my own Dishes so that I can plan meals that may or may not have preparation Recipes.

**Acceptance criteria**
- **AC-33:** Given a user creates a Dish, when it is saved, then it is owned and managed by that user.
- **AC-34:** Given a user manages a Dish, when they associate Recipes with it, then those Recipes are alternative preparations and may be absent.
- **AC-35:** Given a Dish has no associated Recipes, when it is saved, then it has nutrition values for calories, protein, fat, and carbohydrates, expressed per serving.
- **AC-40:** Given a user creates a Dish, when it is saved, then it has a name and an image URL.

**Related domain rules:** R-39, R-40, R-41, R-42, R-46

### US-19 — Plan Dishes and use their nutrition
As a user, I want to plan Dishes and select their preparation when available so that daily nutrition reflects what I plan to eat.

**Acceptance criteria**
- **AC-36:** Given a user adds a Dish to a Meal Plan, when the entry is saved, then it represents one serving.
- **AC-37:** Given a Dish has associated Recipes, when a user plans it, then they select one of its Recipes as the preparation.
- **AC-38:** Given a Dish is planned, when daily nutrition is calculated, then the selected Recipe's nutrition is used if a Recipe was selected; otherwise the Dish's nutrition is used.
- **AC-41:** Given nutrition is unavailable for a planned item, when daily nutrition is calculated, then the total is marked incomplete rather than treating missing values as zero.

**Related domain rules:** R-42, R-43, R-44, R-48

## Product constraints
- Meal Plans are refactored to use Dishes; existing Recipe-based meal-plan behavior is not preserved or transitioned.
- Cookbook behavior is unchanged.
- Recipe.ImageUrl remains optional and separate from the required Dish image; Recipe-step images remain supported.

**Related domain rules:** R-39, R-40, R-42, R-47

## Impact on existing system
Meal Plans change from direct Recipe assignments to Dish assignments. A planned Dish may identify one associated Recipe as the preparation being used, while a standalone Dish uses its own per-serving nutrition. Existing Recipe-based meal-plan data does not need to be migrated or preserved.
