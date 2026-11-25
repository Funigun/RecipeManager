# MVP Specification

## Table of Contents

- [Product purpose](#product-purpose)
- [MVP scope](#mvp-scope)
- [Users and roles](#users-and-roles)
- [Core workflows](#core-workflows)
- [Feature requirements](#feature-requirements)
- [Additional rules and constraints](#additional-rules-and-constraints)
- [Out of scope](#out-of-scope)
- [Open questions](#open-questions)

## Product purpose

Help users organize their ingredients and recipes, collect recipes into cookbooks, plan meals, and create shopping lists.

## MVP scope

The MVP includes the following capabilities:

- **Accounts**
  - Retain registration and the existing Regular/Admin roles.
  - Let account holders change their sign-in email after verifying the new address, recover access with a new password, and change their password.
  - Limit self-service operations to the account holder's own account.
- **Ingredients and shared catalogs**
  - Let admins maintain global ingredients, units, unit conversions, ingredient conversions, ingredient and recipe categories, shopping-list categories, and meal categories.
  - Let users manage private ingredients, optionally map them to global ingredients, and order shopping-list categories for themselves.
  - Require a primary unit for every ingredient; keep classification categories separate from shopping-list categories.
  - Archive ingredients referenced by recipes rather than deleting them.
- **Recipes**
  - Let users manage recipes, including ingredient amounts, serving counts, nutrition, sections, and steps.
  - Retain existing cookbook behavior; this MVP defines no cookbook changes.
- **Dishes**
  - Let users create and manage their own Dishes, each with a required name and image URL.
  - Allow zero or more alternative preparation Recipes. When a Dish has associated Recipes, require the user to select exactly one when planning it.
  - Dish nutrition is optional when Recipes are associated; standalone Dishes require complete per-serving nutrition for calories, protein, fat, and carbohydrates.
- **Meal planning**
  - Let users maintain one plan per calendar day and assign Dishes to global meal categories.
  - Allow multiple Dishes in the same category; each assignment represents one serving.
  - When a Dish has associated Recipes, require the user to select exactly one Recipe being prepared and use its nutrition for the day's calculation. Use a standalone Dish's nutrition when it has no associated Recipes.
- **Shopping lists**
  - Generate lists on demand from a meal plan or directly selected recipes.
  - Use saved recipe amounts for direct selections.
  - For a planned Dish with a selected Recipe, use that Recipe's ingredients and scale quantities by planned servings divided by the Recipe's saved serving count.
  - If no Recipe is selected, list the Dish under the dedicated "Dishes" category, which admins cannot manage.
  - Expand preparation ingredients into their component ingredients.
  - Normalize amounts when conversions are available; otherwise retain original units and do not combine amounts across units.
  - Include information about conversions used and do not persist lists.
- **Data transition**
  - Start the redesigned domain with a clean database.
  - Allow obsolete test data to be discarded without migration or manual recreation.

Friend management, recipe visibility, sharing permissions, and copying recipes between users are not part of this MVP.

## Users and roles

| Role | Capabilities | Restrictions |
|---|---|---|
| Regular user | Manage private ingredients, recipes, Dishes, cookbooks, and meal plans; generate shopping lists; use global catalogs; optionally map private ingredients to global ingredients; order shopping-list categories for themselves; manage their own sign-in email and password and recover their account. | Cannot create or maintain global catalogs or manage another user's account, recipes, Dishes, or permissions. |
| Admin | All regular-user capabilities; create and maintain global ingredients, units and conversions, ingredient and recipe categories, shopping-list categories, and meal categories. | Cannot manage other users' accounts, roles, or permissions through this MVP. |

## Core workflows

1. **Manage an account**
   - A user registers and signs in.
   - The user can change their password or recover access.
   - The user can request a sign-in email change; the current address remains active until the replacement is verified, then the new address becomes active.
2. **Build a personal recipe collection**
   - A user selects shared or private ingredients.
   - The user creates recipes with ingredient amounts and preparation steps.
   - The user organizes recipes in cookbooks.
3. **Manage Dishes**
   - A user creates and manages their own Dishes.
   - A Dish may have zero or more associated Recipes as alternative preparations.
4. **Maintain shared catalogs**
   - An admin manages global ingredients, measurement units and conversions, ingredient and recipe categories, shopping-list categories, and meal categories.
   - Users select from the shared catalogs.
5. **Plan meals and shop**
   - A user assigns Dishes to meal categories on a calendar day and must select exactly one preparation Recipe when one or more are associated.
   - The user generates a shopping list from the plan, using the selected Recipe's ingredients or listing the Dish under the dedicated "Dishes" category.
6. **Generate a list from recipes**
   - A user selects recipes directly.
   - The user generates a shopping list using the recipes' saved ingredient amounts.

## Feature requirements

- **Accounts**
  - Preserve registration and sign-in.
  - Let account holders change their own password; the new password must work for subsequent sign-in.
  - Let account holders recover access by setting a new password that works for subsequent sign-in.
  - Keep the current sign-in email active until the account holder verifies the replacement; only then make the new address active.
- **Ingredient catalogs**
  - Support admin-maintained global ingredients and user-owned private ingredients.
  - Allow a private ingredient to map to a global ingredient; mapped private ingredients use the global ingredient's primary unit and conversions.
  - Show the mapped global ingredient's available conversion information during private-ingredient creation.
  - Retain the admin-managed ingredient and recipe category catalogs.
- **Ingredient organization**
  - Keep ingredient classification and shopping-list grouping as distinct concepts.
  - Require a shopping-list category for ingredients without preparation recipes.
  - Let users order shopping-list categories independently.
- **Units and conversions**
  - Provide global units maintained by admins.
  - Support within-group unit conversions and ingredient-specific conversions from alternate units to a global ingredient's primary unit.
- **Recipes and cookbooks**
  - Let users manage recipes, including ingredient amounts, serving count, nutrition, sections, and steps.
  - Let users organize recipes using existing cookbook functionality.
- **Dishes**
  - Let users create and manage their own Dishes, each with a required name and image URL, optionally associating alternative Recipes.
  - Require standalone Dish nutrition values for calories, protein, fat, and carbohydrates, expressed per serving.
- **Meal planning**
  - Let users create a date-only plan for each calendar day and assign Dishes to global meal categories.
  - Allow multiple Dishes in the same category; each entry represents one serving.
  - For a Dish with associated Recipes, let the user select the Recipe being prepared and use that Recipe's nutrition in the day's calculation. For a Dish without Recipes, use the Dish's nutrition.
  - If nutrition is unavailable for a planned item, mark the daily nutrition total incomplete rather than treating missing values as zero.
- **Shopping lists**
  - Generate lists on demand from meal plans or directly selected recipes.
  - Use saved quantities for directly selected Recipes.
  - For a planned Dish with a selected Recipe, use that Recipe's ingredients and scale quantities by planned servings divided by the Recipe's saved serving count.
  - If no Recipe is selected, list the Dish under the dedicated "Dishes" category, which admins cannot manage.
  - Expand preparation ingredients according to their required output amount.
- **Shopping-list quantities**
  - Normalize amounts when an applicable conversion is available.
  - Otherwise preserve the original unit, mark the amount unconverted, and do not combine it with amounts in other units.
  - Include information about conversions used.
- Each planned Dish entry represents one serving; selected Recipe ingredients are scaled by planned servings divided by the Recipe's saved serving count.

## Additional rules and constraints

- An unmapped private ingredient has no alternate-unit conversions.
- Each unit group has exactly one reference unit, and within-group conversion factors are relative to it. Conversions between unit groups are ingredient-specific, not part of the global unit catalog.
- An ingredient referenced by a recipe remains available to existing recipes when archived and cannot be added to new recipes. It may be permanently deleted once no recipe references it.
- A plan date has no time or timezone, and each Dish entry has exactly one meal category.
- Preparation-recipe quantities scale by the required prepared-ingredient amount divided by the preparation recipe's total output amount.
- Ingredients have no package information.
- Meal plans are refactored to use Dishes without preserving or transitioning existing recipe-based meal-plan behavior.

## Out of scope

- Friend management, recipe visibility levels (private, public, or friends-only), recipe sharing permissions, and copying recipes between users.
- Admin tools for managing user accounts, roles, or permissions; account closure and personal-data deletion.
- User-defined units and custom shopping-list or meal-category catalogs.
- Persisting shopping lists or specifying the detailed presentation of conversion information.
- Migrating or manually recreating obsolete test data for the domain redesign.
- Detailed UI requirements are outside the scope of this product specification; see the separate [UI specification](UI-Specification.md).

## Open questions

- What email delivery approach and verification/password-recovery token lifetimes and security controls are required?
- What are the maximum Dish name length and image URL constraints?
- What performance, availability, hosting, or data-retention targets apply? None are specified in the source PRDs.
