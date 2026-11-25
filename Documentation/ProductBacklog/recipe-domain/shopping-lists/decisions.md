---
status: approved
topic: Domain redesign — Shopping lists
---
# Shopping-List Decisions

## Decisions

### Shopping-list generation

- <u>D-16 — Meal-plan shopping lists use the selected preparation (User)</u>

  **Decision:** For a planned Dish with a selected Recipe, use that Recipe's ingredients and scale quantities by planned servings divided by the Recipe's saved serving count. Each Dish entry represents one serving. If no Recipe is selected, list the Dish under the dedicated "Dishes" category. Admins cannot manage this category.

  **Reason:** Shopping lists reflect the chosen preparation while keeping Dishes without a selected Recipe visible.

- <u>D-17 — Prepared-ingredient shopping behavior (Recommended)</u>

  **Decision:** Continue expanding ingredients that have preparation recipes into their recipe ingredients. Ingredients without preparation recipes appear as shopping-list items and require a shopping-list category.

  **Reason:** This preserves the existing recursive shopping-list behavior and matches the category assignment rule.

- <u>D-18 — Plan-based shopping quantities (User)</u>

  **Decision:** Each Dish entry in a day represents one serving. For a Dish with a selected Recipe, plan-based shopping lists scale the Recipe's quantities to the total planned servings.

  **Reason:** This enables shopping totals to reflect planned servings.

- <u>D-19 — Direct recipe shopping quantities (User)</u>

  **Decision:** When generating a list by selecting a recipe directly, use its saved amounts without scaling.

  **Reason:** There is no plan serving count to scale to, and direct selection should preserve the recipe's saved quantities.

- <u>D-29 — Preparation-recipe expansion (User)</u>

  **Decision:** Scale a preparation recipe's saved ingredient amounts by the required prepared-ingredient amount divided by the preparation recipe's total output amount. Serving count applies to meal portions, not this physical output conversion.

  **Reason:** Proportional scaling preserves quantities when only part of a prepared ingredient is needed.

- <u>D-20 — Shopping-list conversion explanation (User)</u>

  **Decision:** Include supporting information explaining how shopping-list totals were calculated using available conversions; defer the detailed presentation.

  **Reason:** This supports transparency while leaving presentation and persistence choices for later.

- <u>D-28 — Shopping-list persistence (User)</u>

  **Decision:** Shopping lists are derived on demand and are not persisted.

  **Reason:** This matches the current generator and avoids adding list ownership, lifecycle, history, and stale-data rules to the redesign.

- <u>D-32 — Missing ingredient-unit conversion (User)</u>

  **Decision:** Include an amount in its original unit, mark it as unconverted, and do not aggregate it with amounts in another unit when no conversion is available.

  **Reason:** This preserves quantities when conversion data is unavailable and follows the requirement to use available conversions.
