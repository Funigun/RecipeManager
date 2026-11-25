---
status: approved
topic: Domain redesign — Categories
---
# Category Decisions

## Decisions

### Categories and catalogs

- <u>D-04 — Shopping-list categories (User)</u>

  **Decision:** Use a global shopping-list category catalog. An ingredient without a preparation recipe must have an assigned category. Users may order categories independently but cannot create custom shopping-list categories for now.

  **Reason:** This gives users a consistent category set while allowing personal ordering.

- <u>D-25 — Global category catalog ownership (User)</u>

  **Decision:** Admins create and maintain the global shopping-list category catalog and the global meal-category catalog.

  **Reason:** Central administration is consistent with the existing ownership of shared units and ingredient categories.

- <u>D-33 — Ingredient classification (User)</u>

  **Decision:** Keep general ingredient classification categories separate from shopping-list categories.

  **Reason:** The existing model treats `Categories` and `ShoppingListCategoryId` as distinct relationships and purposes.

- <u>D-15 — Meal-planner categories (Recommended)</u>

  **Decision:** Use a global standard catalog of meal categories, including examples such as breakfast, lunch, dinner, and snack.

  **Reason:** This provides predefined categories without requiring user setup.
