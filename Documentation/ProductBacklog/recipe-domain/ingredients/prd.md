---
status: approved
topic: Domain redesign — Ingredients
source: decisions.md
---
# Ingredients

## Summary
Provide a shared global ingredient catalog alongside user-owned private ingredients. A private ingredient may map to a global ingredient to reuse its identity and conversion information.

## Users and permissions
See the shared roles and permissions in the [domain redesign overview](../overview.md#users-and-permissions).

## Domain rules

### Ingredient catalog and lifecycle

- **R-02 — Ingredient catalog ownership** *(Entities: Ingredient; Source: D-02, D-22)*
  - The catalog contains admin-managed global ingredients and user-owned private ingredients.
- **R-03 — Private-to-global ingredient mapping** *(Entities: Private ingredient, global ingredient; Source: D-03)*
  - A private ingredient may optionally be mapped to a global ingredient.
- **R-22 — Global ingredient governance** *(Entities: Admin, user, ingredient; Source: D-22)*
  - Admins maintain global ingredients; users may add private ingredients.
- **R-05 — Global ingredient conversions** *(Entities: Global ingredient, private ingredient, conversion, shopping list; Source: D-05)*
  - Global ingredient conversions are admin-managed.
  - A private ingredient mapped to a global ingredient uses that global ingredient's conversions.
  - Available conversion information is shown during ingredient creation; shopping-list totals explain the conversions used.
- **R-07 — Global ingredient conversion target** *(Entities: Global ingredient, unit conversion; Source: D-07, D-26)*
  - A global ingredient conversion maps an alternate unit directly to that ingredient's primary unit.
  - Cross-group conversions are defined for a specific global ingredient, not by the unit catalog. See [Units](../units/prd.md#domain-rules).
- **R-21 — Ingredient package data** *(Entities: Ingredient; Source: D-21)*
  - Ingredients do not have package information, and users do not define package sizes.
- **R-35 — Unmapped private ingredient conversions** *(Entities: Private ingredient, unit conversion; Source: D-35)*
  - An unmapped private ingredient has no alternate-unit conversions.
- **R-36 — Ingredient primary unit** *(Entities: Ingredient, unit; Source: D-36)*
  - Every ingredient has a primary unit.
- **R-37 — Mapped private ingredient primary unit** *(Entities: Private ingredient, global ingredient, unit; Source: D-37)*
  - A private ingredient mapped to a global ingredient uses that global ingredient's primary unit.
- **R-38 — Ingredient archival and deletion** *(Entities: Ingredient, recipe; Source: Product decision)*
  - An ingredient referenced by any recipe ingredient entry cannot be permanently deleted; it must be archived instead.
  - Archived ingredients remain available to existing recipe references but cannot be added to new recipe ingredient entries.
  - Permanent deletion is allowed once no recipe ingredient entries reference the ingredient.

## User stories

### US-01 — Maintain the global ingredient catalog
As an administrator, I want to maintain shared ingredients so that users can select them from a common catalog.

**Acceptance criteria**
- **AC-01:** Given an administrator creates a global ingredient, when a user browses the catalog, then that ingredient is available as a shared catalog item.

**Related domain rules:** R-02, R-22

### US-02 — Create private ingredients
As a user, I want to create private ingredients so that I can track ingredients that are not in the shared catalog.

**Acceptance criteria**
- **AC-02:** Given a user creates a private ingredient, when the ingredient is saved, then it remains user-owned and may optionally be mapped to a global ingredient.
- **AC-29:** Given a private ingredient is not mapped to a global ingredient, when its available conversions are considered, then it has no alternate-unit conversions.
- **AC-30:** Given an ingredient is created or updated, when its unit is saved, then it has a primary unit; if it is a private ingredient mapped to a global ingredient, then it uses that global ingredient's primary unit.

**Related domain rules:** R-02, R-03, R-35, R-36, R-37

### US-03 — Map a private ingredient to a global ingredient
As a user, I want to map a private ingredient to a global ingredient so that it can use the shared ingredient's conversion information.

**Acceptance criteria**
- **AC-05:** Given a private ingredient is mapped to a global ingredient, when its conversion information is used, then the global ingredient's conversions are used; available conversion information is shown during ingredient creation.

**Related domain rules:** R-03, R-05, R-37

### US-07 — Archive or delete ingredients according to their recipe use
As a user, I want to remove unused ingredients and archive ingredients still used by recipes so that existing recipes remain valid.

**Acceptance criteria**
- **AC-32:** Given a global or private ingredient is referenced by a recipe ingredient entry, when it is removed from active use, then it is archived rather than permanently deleted; existing recipe references remain valid, and the archived ingredient cannot be added to new recipe ingredient entries. When no recipe ingredient entries reference the ingredient, it may be permanently deleted.

**Related domain rules:** R-38

### US-17 — Maintain global ingredient conversions
As an administrator, I want to maintain global ingredient conversions so that quantities can be normalized consistently for each ingredient.

**Acceptance criteria**
- **AC-07:** Given an administrator defines a conversion for a global ingredient, when the conversion is used, then it maps an alternate unit directly to that ingredient's primary unit; if the units belong to different groups, the conversion is specific to that ingredient.

**Related domain rules:** R-05, R-07, R-26 (Units)

## Impact on existing system
The current `Ingredient` model includes nutritional values, a base unit, an optional shopping-list category, package information, ingredient-level unit conversions, classification categories, and recipe links. The target introduces global and private catalog ownership, optional private-to-global mapping, global ingredient conversions, and a required shopping-list category for ingredients without preparation recipes. Package information and ingredient-level nutrition are removed; each ingredient has a primary unit, and a mapped private ingredient uses its global ingredient's primary unit and conversions. Ingredients referenced by recipes are archived rather than permanently deleted, preserving existing recipe references while preventing new references. See [Categories](../categories/prd.md) for category behavior and [Recipes](../recipes/prd.md) for recipe-owned nutrition.

## Documentation targets
- Update the Ingredients explanation in `wiki/domain`.
