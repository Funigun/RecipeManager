---
status: approved
topic: Domain redesign — Dishes
---
# Dish Decisions

## Decisions

### Dishes

- <u>D-10 — Dishes as meal-planning items (User)</u>

  **Decision:** Meal Plans schedule Dishes. A Dish may have zero or more associated Recipes and may have nutritional values.

  **Reason:** Dishes support planning meals independently of a Recipe while allowing optional preparation instructions.

- <u>D-11 — Dish ownership (User)</u>

  **Decision:** Each user creates and manages their own Dishes.

  **Reason:** Personal ownership supports user-specific meal planning.

- <u>D-12 — Associated Recipes are alternative preparations (User)</u>

  **Decision:** Recipes associated with a Dish are alternative ways to prepare it, not components combined into the Dish.

  **Reason:** The Dish remains independently plannable, including when it represents a meal without a Recipe.

- <u>D-13 — Standalone Dish nutrition (User)</u>

  **Decision:** Dish nutrition describes one serving. A Dish without associated Recipes must have nutrition values for calories, protein, fat, and carbohydrates. A Meal-Plan Entry for a Dish represents one serving.

  **Reason:** Complete per-serving nutrition supports calculations for standalone meals.

- <u>D-14 — Nutrition follows the planned preparation (User)</u>

  **Decision:** For a Dish without associated Recipes, use its manually specified nutrition. When planning a Dish with associated Recipes, select the Recipe being used and use that Recipe's nutrition for the day's calculation.

  **Reason:** Daily nutrition reflects the meal or preparation the user plans to have.

- <u>D-15 — Refactor Meal Plans to use Dishes (User)</u>

  **Decision:** Refactor Meal Plans to use Dishes without preserving or transitioning existing Recipe-based meal-plan behavior.

  **Reason:** Existing behavior does not need to be carried forward.

- <u>D-17 — Dish identity and image (User)</u>

  **Decision:** Each Dish has a required name and image URL. The maximum name length and image URL constraints are not specified.

  **Reason:** A Dish needs a clear identity and its own image, separate from any preparation Recipe.

- <u>D-18 — Retain the optional Recipe main image (User)</u>

  **Decision:** Retain the optional main image URL on a Recipe, separate from the required Dish image. Recipe-step images remain supported.

  **Reason:** Recipe imagery remains useful for representing a particular preparation independently of the Dish.

- <u>D-19 — Missing nutrition does not count as zero (User)</u>

  **Decision:** If nutrition is unavailable for a planned item, mark the daily nutrition total incomplete rather than treating missing values as zero.

  **Reason:** An incomplete total is more accurate than a total that silently treats unknown values as zero.

## Out of scope

- Changes to cookbook behavior.
