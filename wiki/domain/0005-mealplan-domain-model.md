# 5. MealPlan Domain Model explanation

Date: 2024-05-23
Status: Accepted

## Context

The `MealPlan` domain model addresses the user need to plan their cooking activities ahead of time. It acts as a scheduling entity that associates specific recipes with a specific date.

Key characteristics:
- **Temporal Association**: Each meal plan is strictly bound to a `Date`. This allows the application to present a calendar view or a "Today's Plan" dashboard.
- **Recipe Collection**: It holds a simple list of recipes intended for that day. It does not strictly define "Breakfast" vs "Dinner" slots internally; rather, it is a bucket of recipes for the day.
- **Simple Lifecycle**: Creation involves picking a date and initial recipes. Updates allow modifying the list of recipes.

For more details on operations, see `Features/MealPlanner`.

## Relationships

- **MealPlan to Recipe**:
    - A conceptual Many-to-Many relationship. A `MealPlan` contains multiple `Recipes`, and a `Recipe` can appear in many `MealPlans` (across different dates or users).
    - **Implementation**: The model stores a list of `RecipeId` value objects.
    - **Configuration**: Mapped via `OwnsMany` in `MealPlanConfiguration` to the `RecipeToMealPlan` table. This indicates that the association is managed by the aggregate root (`MealPlan`), and the specific link exists only within the context of that plan.

- **Storage**:
    - The entity is mapped to the `MealPlans` table.
