---
status: approved
topic: Domain redesign — Meal planning
source: decisions.md
---
# Meal Planning

## Summary
Use one date-only meal plan per user per calendar day. A plan may contain any number of categorized Dish entries, including multiple Dishes assigned to the same meal category.

## Users and permissions
See the shared roles and permissions in the [domain redesign overview](../overview.md#users-and-permissions).

## Domain rules

### Meal planning

- **R-16 — Meal-category assignments** *(Entities: Meal-plan entry, Dish, meal category; Source: D-16)*
  - A day may have multiple Dish assignments in the same meal category.
  - Each Dish assignment has exactly one meal category.
- **R-28 — Meal plans per day** *(Entities: User, meal plan, meal-plan entry; Source: D-30)*
  - Each user has exactly one meal plan per calendar day; a plan contains any number of categorized Dish entries.
- **R-29 — Meal-plan date** *(Entities: Meal plan; Source: D-31)*
  - A meal-plan date represents a calendar date only, with no time or timezone.

Meal categories use the global catalog described in [Categories](../categories/prd.md#domain-rules) (R-15).
Dish ownership, preparation selection, and nutrition behavior are described in [Dishes](../dishes/prd.md#domain-rules).

## User stories

### US-10 — Plan Dishes by meal category
As a user, I want to assign Dishes to meal categories on each day so that I can plan meals by time of day and include multiple Dishes in a category.

**Acceptance criteria**
- **AC-16:** Given a day has a meal plan, when a Dish is assigned to it, then the assignment has exactly one category from the global meal-category catalog and represents one serving.
- **AC-17:** Given a day already has a Dish in a category, when another Dish is assigned to that same category, then both assignments are retained.
- **AC-25:** Given a user creates or updates a meal plan, when the plan date is saved, then the user has no more than one plan for that calendar date and the date has no time or timezone component.

**Related domain rules:** R-15 (Categories), R-16, R-28, R-29

## Impact on existing system
The current `MealPlan` stores a timestamp and Recipe references without meal categories. The target uses one date-only plan per user per day, containing any number of Dish entries, each assigned to one global meal category and representing one serving. A Dish with associated Recipes identifies the selected preparation; nutrition behavior is defined in the [Dishes PRD](../dishes/prd.md).

## Documentation targets
- Update the MealPlan explanation in `wiki/domain` for categories, entries, and dates.
- Create or update `wiki/adr` records for significant meal-planning choices after deferred and open questions are resolved.
