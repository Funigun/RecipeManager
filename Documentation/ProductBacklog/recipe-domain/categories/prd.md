---
status: approved
topic: Domain redesign — Categories
source: decisions.md
---
# Categories

## Summary
Use globally administered shopping-list and meal-category catalogs while keeping ingredient classification separate. Users may personalize their shopping-list category order, but cannot create or modify global catalog entries.

## Users and permissions
See the shared roles and permissions in the [domain redesign overview](../overview.md#users-and-permissions).

## Domain rules

### Categories and catalogs

- **R-04 — Shopping-list categories** *(Entities: Ingredient, shopping-list category; Source: D-04, D-25)*
  - Shopping-list categories come from an admin-maintained global catalog.
  - An ingredient without a preparation recipe must have an assigned shopping-list category.
  - Each user may order the categories independently; users cannot create custom shopping-list categories.
- **R-15 — Meal-category catalog** *(Entities: Meal-plan entry, meal category; Source: D-15, D-25)*
  - Meal planning uses an admin-maintained global standard catalog of meal categories, including categories such as breakfast, lunch, dinner, and snack.
- **R-25 — Global category catalog ownership** *(Entities: Admin, user, shopping-list category, meal category; Source: D-25)*
  - Admins create and maintain the global shopping-list and meal-category catalogs.
  - Regular users cannot modify either catalog.
- **R-33 — Ingredient classification categories** *(Entities: Ingredient, classification category, shopping-list category; Source: D-33)*
  - Ingredient classification categories and shopping-list categories are separate concepts and catalogs.

## User stories

### US-04 — Categorize ingredients for shopping lists
As a user, I want ingredients without preparation recipes to have a shopping-list category so that they can be included in shopping lists.

**Acceptance criteria**
- **AC-03:** Given an ingredient has no preparation recipe, when it is used for a shopping list, then it has an assigned category from the global shopping-list catalog.

**Related domain rules:** R-04, R-17 (Shopping lists)

### US-05 — Personalize shopping-list category order
As a user, I want to order shopping-list categories for myself so that lists reflect my preferences without changing another user's order.

**Acceptance criteria**
- **AC-04:** Given a user changes their ordering of shopping-list categories, when they view their ordering, then it reflects their preference without changing another user's ordering.

**Related domain rules:** R-04

### US-06 — Keep ingredient classification separate from shopping-list categories
As a user, I want ingredient classification and shopping-list grouping to remain separate so that each serves its distinct purpose.

**Acceptance criteria**
- **AC-28:** Given an ingredient has classification categories and a shopping-list category, when it is classified and grouped on a shopping list, then the classification categories and shopping-list category remain separate and neither substitutes for the other.

**Related domain rules:** R-33

### US-16 — Maintain global category catalogs
As an administrator, I want to maintain global shopping-list and meal-category catalogs so that users share consistent categories.

**Acceptance criteria**
- **AC-23:** Given an administrator adds a category to either global catalog, when a regular user selects a shopping-list or meal category, then the added category is available; regular users cannot add or modify entries in either global catalog.

**Related domain rules:** R-15, R-25

## Impact on existing system
General ingredient classification categories remain separate from shopping-list categories. Shopping-list categories retain per-user ordering, while both shopping-list and meal-category catalogs are globally maintained.

## Documentation targets
- Update the Ingredients and shopping-list category explanations in `wiki/domain`.
