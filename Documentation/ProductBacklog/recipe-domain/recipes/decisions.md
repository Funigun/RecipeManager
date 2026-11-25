---
status: approved
topic: Domain redesign — Recipes
---
# Recipe Decisions

## Decisions

### Recipes and product scope

- <u>D-13 — Recipe visibility (User)</u>

  **Decision:** Defer recipe visibility; recipes will not have visibility levels in the current target.

  **Reason:** The user later deferred recipe visibility, superseding the earlier approved scope.

- <u>D-14 — Recipe sharing and copies (User)</u>

  **Decision:** Defer friend sharing, permissions, and independent recipe copies; they are out of scope for this redesign.

  **Reason:** The user later deferred recipe sharing and copies, superseding the earlier approved scope.

- <u>D-27 — Recipe serving count (User)</u>

  **Decision:** A recipe's saved ingredient amounts make its stated number of servings. Plan-based quantities scale by planned servings divided by the saved serving count.

  **Reason:** This defines the recipe yield and provides the denominator for plan-based shopping-list scaling and preparation-recipe expansion.

- <u>D-34 — Nutrition ownership (User)</u>

  **Decision:** Nutrition belongs to recipes only; remove ingredient-level nutrition.

  **Reason:** This matches the proposed domain shape and keeps nutrition attached to the recipe.
