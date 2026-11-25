---
status: approved
topic: Domain redesign — Ingredients
---
# Ingredient Decisions

## Decisions

### Ingredients

- <u>D-02 — Ingredient catalog ownership (User)</u>

  **Decision:** Provide a global shared ingredient catalog and allow users to add private ingredients.

  **Reason:** This reduces duplication without preventing users from representing ingredients absent from the shared catalog; sharing private ingredients may require recipient-side handling.

- <u>D-03 — Custom-to-global ingredient mapping (User)</u>

  **Decision:** A user may optionally map a private ingredient to a global ingredient.

  **Reason:** The mapping provides a way to relate a private ingredient to the shared catalog.

- <u>D-22 — Global ingredient governance (Recommended)</u>

  **Decision:** Admins create and maintain global ingredients; users can add private ingredients.

  **Reason:** This provides a single source of truth for shared ingredient conversions and identity.

- <u>D-05 — Global ingredient conversions (User)</u>

  **Decision:** Add an admin-managed global ingredient conversion model, analogous to the current ingredient-unit conversions. A private ingredient mapped to a global ingredient uses the global conversions. Show available conversion information during ingredient creation and explain the conversions used in shopping-list totals.

  **Reason:** This allows shopping lists to normalize mapped ingredients without requiring users to define conversions.

- <u>D-07 — Global ingredient conversion target (Recommended)</u>

  **Decision:** Each global ingredient conversion maps an alternate unit directly to that ingredient's primary unit.

  **Reason:** Direct mappings align with shopping-list normalization and avoid ambiguous conversion paths.

- <u>D-21 — Ingredient package data (User)</u>

  **Decision:** Remove package information from ingredients; users do not define package sizes.

  **Reason:** The target state explicitly removes package data to avoid user-maintained package definitions.

- <u>D-36 — Primary unit (User)</u>

  **Decision:** Every ingredient must have a primary unit.

  **Reason:** The user confirmed that a primary unit is required.

- <u>D-37 — Mapped private primary unit (User)</u>

  **Decision:** A mapped private ingredient must use its mapped global ingredient's primary unit.

  **Reason:** This lets the private ingredient use the mapped global ingredient's conversion table consistently.

- <u>D-35 — Unmapped private conversions (User)</u>

  **Decision:** An unmapped private ingredient has no alternate-unit conversions.

  **Reason:** The user selected no conversions rather than user-defined private conversions.
