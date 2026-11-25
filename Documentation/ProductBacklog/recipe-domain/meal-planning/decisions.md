---
status: approved
topic: Domain redesign — Meal planning
---
# Meal Planning Decisions

## Decisions

### Meal planning

- <u>D-16 — Meal-plan category assignments (Recommended)</u>

  **Decision:** A day may have multiple Dishes assigned to the same meal category; each Dish assignment has one category.

  **Reason:** This supports multi-Dish meals without introducing separate meal-slot entities.

- <u>D-30 — Meal plans per day (User)</u>

  **Decision:** Each user has exactly one meal plan per calendar day; a plan contains any number of categorized Dish entries.

  **Reason:** This matches the day-level plan model and keeps multiple recipes in a category as entries rather than separate plans.

- <u>D-31 — Meal-plan date (User)</u>

  **Decision:** Represent the plan date as a date-only calendar value, with no time or timezone.

  **Reason:** Plans represent calendar days rather than instants; date-only semantics avoid timezone shifts.
