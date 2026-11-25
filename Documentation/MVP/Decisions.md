# Recipe Manager MVP Decisions

This register records decisions and proposals that shape the MVP. The Specification and Domain documents describe the resulting requirements and model; consequential architecture decisions link to an ADR for detail.

## Table of contents

- [Identity and Access](#identity-and-access)
  - [D-01: ASP.NET Core Identity for account and role management](#d-01-aspnet-core-identity-for-account-and-role-management)
  - [D-02: Self-service account management](#d-02-self-service-account-management)
- [Catalog and Ingredients](#catalog-and-ingredients)
  - [D-03: Global and private ingredient catalogs](#d-03-global-and-private-ingredient-catalogs)
  - [D-04: Unit and ingredient conversion model](#d-04-unit-and-ingredient-conversion-model)
  - [D-05: Archive ingredients referenced by recipes](#d-05-archive-ingredients-referenced-by-recipes)
- [Meal Planning](#meal-planning)
  - [D-06: One date-only meal plan per user per day](#d-06-one-date-only-meal-plan-per-user-per-day)
- [Dishes](#dishes)
  - [D-10: Dishes are first-class meal-planning items](#d-10-dishes-are-first-class-meal-planning-items)
  - [D-11: Dishes are user-owned](#d-11-dishes-are-user-owned)
  - [D-12: Associated Recipes are alternative preparations](#d-12-associated-recipes-are-alternative-preparations)
  - [D-13: Standalone Dishes have complete per-serving nutrition](#d-13-standalone-dishes-have-complete-per-serving-nutrition)
  - [D-14: Nutrition follows the planned preparation](#d-14-nutrition-follows-the-planned-preparation)
  - [D-15: Meal plans refactor to use Dishes](#d-15-meal-plans-refactor-to-use-dishes)
- [Shopping Lists](#shopping-lists)
  - [D-07: Derive shopping lists on demand](#d-07-derive-shopping-lists-on-demand)
  - [D-16: Meal-plan shopping lists use the selected preparation](#d-16-meal-plan-shopping-lists-use-the-selected-preparation)
- [Data Transition](#data-transition)
  - [D-08: Start with a clean database](#d-08-start-with-a-clean-database)
- [Architecture](#architecture)
  - [D-09: Modular monolith module boundaries](#d-09-modular-monolith-module-boundaries)

## Statuses

- **Draft**: Still being considered; may change.
- **Proposed**: Agreed in principle but not yet approved.
- **Approved**: Confirmed and ready to implement.
- **Rejected**: Not accepted and will not be implemented.
- **Superseded**: Replaced by a newer decision.
- **Deprecated**: No longer relevant and should not be used.
- **Implemented**: Implemented in code and in use.

## Identity and Access

### D-01: ASP.NET Core Identity for account and role management

**Type:** Technical

**Status:** Implemented

**Context:** The Identity API needs account, credential, and role-management capabilities. Identity and account infrastructure are not the product's focus.

**Decision:** Use ASP.NET Core Identity for account and role management. Retain the Identity API's JWT bearer validation and application-specific access- and refresh-token issuance flows.

**Reason:** Reuse the framework's identity and account-management foundation rather than building those capabilities from scratch, keeping project effort focused on recipe management.

**Consequences:**

- Account and role management use ASP.NET Core Identity conventions and services.
- JWT token issuance, refresh-token behavior, and application authorization policies remain application responsibilities.
- This decision does not imply that all authentication and authorization behavior is supplied by ASP.NET Core Identity.

**Related:** [MVP Specification](Specification.md); [User](../../RecipeManager.Identity.API/Domain/User.cs); [Identity dependency injection](../../RecipeManager.Identity.API/Presentation/DependencyInjection.cs); [AuthorizationService](../../RecipeManager.Identity.API/Features/Common/AuthorizationService.cs).

### D-02: Self-service account management

**Type:** Product

**Status:** Approved

**Context:** Account holders need to maintain access to their own accounts without introducing account-administration features for this MVP.

**Decision:** Preserve registration and the Regular/Admin roles. Let account holders change their own password, recover access with a new password, and change their sign-in email only after verifying the replacement. Keep the current email active while a replacement is pending. Exclude admin management of users, roles, and permissions, account closure, and personal-data deletion.

**Reason:** Prioritize the account holder's essential access and security needs while deferring broader account administration and data-lifecycle decisions.

**Related:** [MVP Specification](Specification.md); [Identity domain](../../wiki/domain/identity.md).

## Catalog and Ingredients

### D-03: Global and private ingredient catalogs

**Type:** Product / Domain

**Status:** Approved

**Context:** Users need shared ingredients while retaining the ability to represent ingredients that are not in the shared catalog.

**Decision:** Admins maintain global ingredients and shared catalogs. Users may create private ingredients and optionally map them to global ingredients. Ingredient classification categories remain distinct from global shopping-list categories; users may order the shared shopping-list categories independently.

**Reason:** Shared catalogs provide consistency without preventing users from maintaining personal ingredients and preferences.

**Related:** [MVP Specification](Specification.md); [MVP Domain](Domain.md).

### D-04: Unit and ingredient conversion model

**Type:** Domain

**Status:** Approved

**Context:** Shopping-list quantities need consistent normalization without assuming every unit-group relationship applies to every ingredient.

**Decision:** Units are global. Each unit group has one reference unit, and within-group conversion factors are relative to it. Conversions across groups are specific to a global ingredient and map an alternate unit directly to its primary unit. A mapped private ingredient uses its global ingredient's primary unit and conversions; an unmapped private ingredient has no alternate-unit conversions.

**Reason:** Group-relative factors support conversions within a measurement group, while ingredient-specific conversions avoid assuming universal conversions across groups.

**Related:** [MVP Specification](Specification.md); [MVP Domain](Domain.md); [Units domain](../../wiki/domain/units.md); [Ingredients domain](../../wiki/domain/ingredients.md).

### D-05: Archive ingredients referenced by recipes

**Type:** Domain

**Status:** Approved

**Context:** Permanently removing an ingredient referenced by a recipe would invalidate that recipe's existing ingredient entry.

**Decision:** Archive a referenced ingredient instead of permanently deleting it. Existing recipe references remain valid, archived ingredients cannot be added to new recipes, and permanent deletion is allowed once no recipe ingredient entries reference the ingredient.

**Reason:** Preserve existing recipe meaning while allowing unused ingredients to be removed permanently.

**Related:** [MVP Specification](Specification.md); [MVP Domain](Domain.md); [Ingredients domain](../../wiki/domain/ingredients.md).

## Meal Planning

### D-06: One date-only meal plan per user per day

**Type:** Product / Domain

**Status:** Approved

**Context:** Meal planning represents a user's calendar day and must support multiple dishes in one meal category.

**Decision:** Each user has one meal plan per calendar day. The plan date has no time or timezone. Multiple entries may use the same global meal category. D-10 and D-13 define the assigned item and serving behavior.

**Reason:** A day-level plan keeps planning simple while allowing a meal to contain multiple entries in one category.

**Related:** [MVP Specification](Specification.md); [MVP Domain](Domain.md); [Meal Planner domain](../../wiki/domain/meal-planner.md).

## Dishes

### D-10: Dishes are first-class meal-planning items

**Type:** Product / Domain

**Status:** Approved

**Context:** Users need to plan meals that may not have a Recipe, while still being able to plan a Dish with alternative preparations.

**Decision:** Meal Plans schedule Dishes. A Dish may have zero or more associated Recipes and may have nutritional values.

**Reason:** Dishes support planning meals independently of a Recipe while allowing optional preparation instructions.

**Related:** [MVP Specification](Specification.md); [MVP Domain](Domain.md).

### D-11: Dishes are user-owned

**Type:** Product / Domain

**Status:** Approved

**Context:** Users need personal meal-planning items.

**Decision:** Each user creates and manages their own Dishes.

**Reason:** Personal ownership supports user-specific meal planning.

**Related:** [MVP Specification](Specification.md); [MVP Domain](Domain.md).

### D-12: Associated Recipes are alternative preparations

**Type:** Domain

**Status:** Approved

**Context:** A Dish may have multiple associated Recipes.

**Decision:** Recipes associated with a Dish are alternative ways to prepare it, not components combined into the Dish.

**Reason:** The Dish remains independently plannable, including when it represents a meal without a Recipe.

**Related:** [MVP Specification](Specification.md); [MVP Domain](Domain.md).

### D-13: Standalone Dishes have complete per-serving nutrition

**Type:** Domain

**Status:** Approved

**Context:** Standalone Dishes need nutrition for meal-plan calculations.

**Decision:** Dish nutrition describes one serving. A Dish without associated Recipes must have nutrition values for calories, protein, fat, and carbohydrates. A Meal-Plan Entry for a Dish represents one serving.

**Reason:** Complete per-serving nutrition supports calculations for standalone meals.

**Related:** [MVP Specification](Specification.md); [MVP Domain](Domain.md).

### D-14: Nutrition follows the planned preparation

**Type:** Product / Domain

**Status:** Approved

**Context:** A Dish may have multiple alternative preparation Recipes or no Recipe.

**Decision:** For a Dish without associated Recipes, use its manually specified nutrition. When planning a Dish with associated Recipes, the user must select exactly one Recipe being prepared and use that Recipe's nutrition for the day's calculation.

**Reason:** Daily nutrition reflects the meal or preparation the user plans to have.

**Related:** [MVP Specification](Specification.md); [MVP Domain](Domain.md).

### D-15: Meal plans refactor to use Dishes

**Type:** Product / Data Transition

**Status:** Approved

**Context:** Meal Plans are changing from Recipe assignments to Dish assignments as part of the domain refactor.

**Decision:** Refactor Meal Plans to use Dishes without preserving or transitioning existing Recipe-based meal-plan behavior.

**Reason:** The user specified that existing behavior does not need to be carried forward.

**Related:** [MVP Specification](Specification.md); [MVP Domain](Domain.md).

### D-17: Dish identity and image

**Type:** Product / Domain

**Status:** Approved

**Context:** Users need to identify and display their personal meal-planning items.

**Decision:** Each Dish has a required name and image URL. The maximum name length and image URL constraints are not specified.

**Reason:** A Dish needs a clear identity and its own image, separate from any preparation Recipe.

**Related:** [MVP Domain](Domain.md).

### D-18: Retain the optional Recipe main image

**Type:** Product / Domain

**Status:** Approved

**Context:** A Dish has its own required image, while Recipes and individual Recipe steps may also have imagery.

**Decision:** Retain the optional main image URL on a Recipe, separate from the required Dish image. Recipe-step images remain supported.

**Reason:** Recipe imagery remains useful for representing a particular preparation independently of the Dish.

**Related:** [MVP Domain](Domain.md); [Recipes domain](../../wiki/domain/recipes.md).

### D-19: Missing nutrition does not count as zero

**Type:** Product / Domain

**Status:** Approved

**Context:** Daily nutrition totals must not imply that missing nutritional information represents zero calories or macros.

**Decision:** If nutrition is unavailable for a planned item, mark the daily nutrition total incomplete rather than treating missing values as zero.

**Reason:** An incomplete total is more accurate than a total that silently treats unknown values as zero.

**Related:** [MVP Domain](Domain.md); [Meal Planner domain](../../wiki/domain/meal-planner.md).

## Shopping Lists

### D-07: Derive shopping lists on demand

**Type:** Product / Domain

**Status:** Approved

**Context:** Shopping lists need to be generated on demand without introducing list persistence and lifecycle management.

**Decision:** Generate shopping lists on demand and do not persist them. Use saved quantities unchanged for directly selected Recipes. Expand preparation ingredients by scaling their Recipe's saved amounts by the required prepared-ingredient amount divided by that Recipe's total output amount. Normalize amounts when an applicable conversion exists; otherwise retain and mark the original amount and unit without combining it with amounts in other units. Include information about conversions used. D-16 defines how meal-plan Dish entries contribute to shopping lists.

**Reason:** Keep lists derived and avoid stored-list ownership, history, and stale-data concerns.

**Related:** [MVP Specification](Specification.md); [MVP Domain](Domain.md); [Recipes domain](../../wiki/domain/recipes.md); [Meal Planner domain](../../wiki/domain/meal-planner.md).

### D-16: Meal-plan shopping lists use the selected preparation

**Type:** Product / Domain

**Status:** Approved

**Context:** A planned Dish may have a selected preparation Recipe, or may have no selected Recipe.

**Decision:** For a planned Dish with a selected Recipe, use that Recipe's ingredients and scale quantities by planned servings divided by the Recipe's saved serving count. Each Dish entry represents one serving. If no Recipe is selected, list the Dish under the dedicated "Dishes" category. Admins cannot manage this category.

**Reason:** Shopping lists reflect the chosen preparation while keeping Dishes without a selected Recipe visible.

**Related:** [MVP Specification](Specification.md); [MVP Domain](Domain.md).

## Data Transition

### D-08: Start with a clean database

**Type:** Product / Data Transition

**Status:** Approved

**Context:** Existing data is obsolete test data and should not constrain the redesigned domain.

**Decision:** Start the redesigned domain with a clean database. Obsolete test data may be discarded; migration or manual recreation is not required.

**Reason:** Avoid spending effort preserving data that has no ongoing product value.

**Related:** [MVP Specification](Specification.md).

## Architecture

### D-09: Modular monolith module boundaries

**Type:** Technical

**Status:** Proposed

**Context:** The domain redesign spans interconnected catalog, recipe, and planning capabilities, while Identity currently runs as a separate API.

**Decision:** The proposed architecture is a modular monolith with Catalog, Recipes, and Planning modules in one Recipe API deployment and database; Identity remains a separate API.

**Reason:** The proposed boundaries aim to reduce coupling while preserving a single deployment and database for strongly connected domain behavior.

**Consequences:** Module ownership, contracts, dependency direction, and enforcement details remain as described in the ADR. This proposal is not an approved implementation decision.

**Related:** [ADR 0001: Modular monolith module boundaries](../../wiki/adr/0001-modular-monolith-module-boundaries.md).