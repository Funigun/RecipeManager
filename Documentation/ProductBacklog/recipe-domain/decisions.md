---
status: approved
topic: Domain redesign
---
# Domain Redesign Decisions

## Table of contents

- [Scope and transition](#scope-and-transition)
- [Ingredients](ingredients/decisions.md)
- [Units](units/decisions.md)
- [Categories](categories/decisions.md)
- [Dishes](dishes/decisions.md)
- [Recipes](recipes/decisions.md)
- [Meal planning](meal-planning/decisions.md)
- [Shopping lists](shopping-lists/decisions.md)

## Decisions

### Scope and transition

- <u>D-01 — Session scope (User)</u>

  **Decision:** Define the minimum domain changes needed to prepare for the target state. Exclude the UI rebuild; treat Azure learning as a separate topic.

  **Reason:** This is the stated goal in `RecipeManager.API/Domain/DomainTarget.md`.

- <u>D-23 — Legacy data migration (User)</u>

  **Decision:** Start from a clean database. Obsolete test data may be discarded; migration or manual recreation is not required.

  **Reason:** The existing data is only obsolete test data.

## Open questions

## Assumptions
- The existing UI is not a constraint because it will be rebuilt.
- Azure cloud learning is out of scope for this domain decision session.
- Shopping-list amounts normalize to global primary units when conversions are available; otherwise, they remain in their original units and are not combined across units.
- Recipes own their ingredient lists, serving counts, nutrition, sections, and steps.
- A recipe's saved ingredient amounts produce its stated number of servings; plan-based quantities scale by planned servings divided by that saved serving count.
- Shopping lists recursively expand ingredients with preparation recipes; preparation quantities scale by the required output amount.

## Out of scope
- UI design and implementation.
- Azure cloud learning and implementation.
- Recipe visibility, friend sharing/permissions, and recipe-copy behavior for this redesign.

## Deferred topics (planned features)
- Recipe visibility, friend sharing/permissions, and recipe-copy behavior.
- Detailed presentation and persistence of shopping-list conversion breakdowns.

## Glossary
- **Global ingredient**: An admin-managed ingredient in the shared catalog.
- **Private ingredient**: A user-owned ingredient that may optionally map to a global ingredient.
- **Classification category**: General ingredient classification, such as Vegetables or Dairy.
- **Shopping-list category**: Category used to group ingredients in a shopping list.
- **Unit-group reference unit**: The unit within a group against which the group's other units define their conversion factors.
- **Meal-plan entry**: One serving of a Dish assigned to one global meal category.
