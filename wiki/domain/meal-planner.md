# Meal planner and shopping lists domain

## Purpose
Meal plans assign Dishes to global meal categories on calendar days. Shopping lists are derived on demand from a plan or directly selected Recipes.

## Entities and value objects
| Name | Kind (entity/value object) | Description | Key attributes |
|------|----------------------------|-------------|----------------|
| Meal plan | Entity | **Changed:** one user's plan for one calendar day, containing categorized Dish entries. | Owner, date-only value, entries |
| Meal-plan entry | Entity | **Changed:** one assignment of a Dish to a meal category; each entry represents one serving. If the Dish has associated Recipes, the entry selects exactly one preparation Recipe. Multiple entries may use the same category. | Dish, optional selected Recipe, meal category |
| Meal category | Entity | **Added:** admin-maintained global catalog item (e.g. breakfast, lunch, dinner, snack). | Name |
| Shopping list | Derived result | **Changed:** generated on demand and not persisted; its source is a plan or direct Recipe selection. | Items, source mode |
| Shopping-list item | Value object | An ingredient amount normalized to its applicable global primary unit when a conversion is available; otherwise retained in its original unit and marked unconverted. | Ingredient, applicable shopping-list category, amount, unit, conversion support or unconverted status |

## Relationships
```mermaid
erDiagram
    USER ||--o{ MEAL_PLAN : owns
    MEAL_PLAN ||--o{ MEAL_PLAN_ENTRY : contains
    MEAL_PLAN_ENTRY }o--|| DISH : assigns
    MEAL_PLAN_ENTRY }o--o| RECIPE : "selected preparation"
    MEAL_PLAN_ENTRY }o--|| MEAL_CATEGORY : "exactly one"
    SHOPPING_LIST ||--o{ SHOPPING_LIST_ITEM : has
    SHOPPING_LIST_ITEM }o--|| INGREDIENT : of
    SHOPPING_LIST_ITEM }o--o| SHOPPING_LIST_CATEGORY : "when not prepared"
```

Each user has exactly one meal plan per calendar date. A date has no time or timezone. Each entry represents one Dish serving and has exactly one meal category. If its Dish has associated Recipes, the entry must select one. A selected Recipe supplies the nutrition for that planned item; a standalone Dish uses its own per-serving nutrition. If nutrition is unavailable, daily totals are marked incomplete rather than treating it as zero.

For shopping lists, a selected preparation Recipe contributes its ingredients, scaled by planned servings divided by the Recipe's saved serving count. A standalone Dish appears under the dedicated "Dishes" category, which admins cannot manage. Ingredients with preparation recipes expand recursively into their recipe ingredients; ingredients without preparation recipes remain shopping-list items and require a shopping-list category. Category ordering is user-specific.

## Invariants and rules
| Rule | Applies to | PRD ref |
|------|------------|---------|
| Meal categories are from an admin-maintained global catalog; regular users cannot modify it. | Meal category | R-15, R-25 |
| A plan may contain any number of Dish entries, including multiple entries in one category; each entry has exactly one category. | Meal plan, entry | R-16, R-28 |
| Each entry represents one serving of a Dish. If the Dish has associated Recipes, the entry selects exactly one. | Meal-plan entry, Dish, Recipe | R-42, R-43 |
| Each user has exactly one plan per calendar date, and a plan date has no time or timezone. | Meal plan | R-28, R-29 |
| A selected Recipe supplies the nutrition for a planned Dish; a standalone Dish supplies its own per-serving nutrition. Missing nutrition marks daily totals incomplete rather than counting as zero. | Meal-plan entry, Dish, Recipe | R-41, R-43, R-44, R-48 |
| Plan-based lists use a Dish's selected Recipe and scale its saved amounts by planned servings divided by saved Recipe servings; each plan entry contributes one serving. | Shopping list | R-18, R-27 |
| A standalone Dish appears under the system-managed "Dishes" shopping-list category. | Shopping list, Dish | D-16 |
| Directly selected recipes contribute their saved amounts without scaling. | Shopping list | R-19 |
| Ingredients with preparation recipes expand recursively; each preparation recipe scales by required output amount divided by its total output amount. | Shopping list, ingredient, recipe | R-17, R-30 |
| Amounts normalize to the applicable global ingredient's primary unit when a conversion is available. If no conversion is available, retain the original amount and unit, mark it unconverted, and do not aggregate it with amounts in another unit. | Shopping-list item | R-20, R-32 |
| A mapped private ingredient uses its global ingredient's conversions; an unmapped private ingredient has no alternate-unit conversions. | Shopping-list item, ingredient | R-05, R-35, R-37 |
| Ingredients without preparation recipes require a shopping-list category from the admin-maintained global catalog; each user's category ordering is independent. | Shopping-list item, shopping-list category | R-04, R-25 |
| Shopping lists include supporting information explaining conversions used; detailed presentation is deferred. | Shopping list | R-05, R-20 |
| Shopping lists are derived on demand and are not persisted. | Shopping list | R-31 |

## Lifecycle / states
Shopping lists are derived results, regenerated on demand from a plan or direct Recipe selection. Conversion support accompanies the result; its detailed presentation remains deferred.

## Open questions
None identified in the approved PRD for this domain.
