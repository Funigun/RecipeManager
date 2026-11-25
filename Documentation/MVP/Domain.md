# MVP Domain

## Table of Contents

- [Unit](#unit)
- [Aggregates](#aggregates)
  - [Global Ingredient](#global-ingredient-aggregate)
  - [Private Ingredient](#private-ingredient-aggregate)
  - [Ingredient Classification Category](#ingredient-classification-category)
  - [Shopping-List Category](#shopping-list-category)
  - [Recipe](#recipe-aggregate)
  - [Dishes](#dishes-aggregate)
  - [Recipe Category](#recipe-category-aggregate)
  - [Cookbook](#cookbook-aggregate)
  - [Meal Plan](#meal-plan-aggregate)
  - [Meal Category](#meal-category-aggregate)
- [Shopping-list derivation](#shopping-list-derivation)
- [Identity domain](#identity-domain)
- [Cross-aggregate references](#cross-aggregate-references)
- [Open questions](#open-questions)

## Unit

### Purpose

A globally available, admin-maintained measurement unit.

### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the unit | Yes | Unique |
| `Name` | Singular display name | Yes | Unique; maximum 50 characters |
| `ShortName` | Optional abbreviated name | No | Unique when present; maximum 25 characters |
| `PluralName` | Plural display name | Yes | Maximum 55 characters |
| `PluralShortName` | Optional plural abbreviation | No | Maximum 30 characters |
| `Group` | Unit's group | Yes | Classification: `Weight`, `Volume`, or `Quantity`; not a separate entity |
| `IsReferenceUnit` | Indicates that this is the reference unit for its group | Yes | Boolean |
| `ConversionFactor` | Factor expressing this unit relative to its group's reference unit | Yes | Numeric representation and exact semantics are open questions |

### Relationships

- Each Unit has exactly one group classification: `Weight`, `Volume`, or `Quantity`.
- Each group classification has exactly one reference Unit; the classification is not a separate entity.
- A cross-group conversion is specific to a global ingredient and maps an alternate unit to that ingredient's primary unit.

### Rules

- Units are global and maintained by admins.
- The Unit catalog does not define conversions between different groups.
- Within-group conversion factors are expressed relative to the group's reference Unit. Their numeric representation and exact direction remain open questions.

## Aggregates

### Global Ingredient Aggregate

#### Global Ingredient (Aggregate Root)

##### Purpose

A shared catalog ingredient maintained by admins.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the ingredient | Yes | Unique |
| `Name` | Display name | Yes | Maximum 100 characters |
| `PrimaryUnit` | Unit used as the ingredient's primary unit | Yes | References one Unit |
| `Status` | Whether the ingredient is available for new use | Yes | `Active` or `Archived` |

##### Relationships

- Has zero or more ingredient classification categories.
- May have one shopping-list category.
- May be produced by one preparation recipe.
- Contains zero or more Global Ingredient Conversions.
- May be referenced by recipe ingredient entries.

##### Rules

- Admins maintain the global ingredient catalog.
- If it has no preparation recipe, it must have a shopping-list category from the global catalog maintained by admins.
- Ingredient classification categories and shopping-list categories are separate.
- Ingredient data has no package information or ingredient-level nutrition.
- If recipe ingredient entries reference it, it must be archived rather than deleted. Existing entries remain valid, archived ingredients cannot be added to new entries, and permanent deletion is allowed once no entries reference it.

#### Global Ingredient Conversion (Child Entity)

##### Purpose

A conversion from an alternate Unit to the owning Global Ingredient's primary Unit.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the conversion within its owning aggregate | Yes | Unique |
| `AlternateUnit` | Unit being converted from | Yes | References one Unit |
| `Ratio` | Amount ratio to the owning ingredient's primary unit | Yes | Numeric; precision not specified |

##### Relationships

- Belongs to one Global Ingredient.
- References one Unit for the alternate unit.

##### Rules

- Maps directly to its owning Global Ingredient's primary unit.

### Private Ingredient Aggregate

#### Private Ingredient (Aggregate Root)

##### Purpose

An ingredient owned by one user for their private catalog.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the ingredient | Yes | Unique |
| `Name` | Display name | Yes | Maximum 100 characters |
| `PrimaryUnit` | Unit used as the ingredient's primary unit | Yes | References one Unit |
| `MappedGlobalIngredient` | Optional global ingredient this private ingredient uses | No | References one Global Ingredient aggregate |
| `Status` | Whether the ingredient is available for new use | Yes | `Active` or `Archived` |

##### Relationships

- References one owning User aggregate.
- May have zero or more ingredient classification categories.
- May have one shopping-list category.
- May be produced by one Recipe aggregate.
- May be referenced by Recipe Ingredient entities.

##### Rules

- A private ingredient may optionally map to a Global Ingredient aggregate.
- If mapped, it uses that global ingredient's primary unit and conversions. If unmapped, it has no alternate-unit conversions.
- If it has no preparation recipe, it must have a shopping-list category from the global catalog maintained by admins.
- Ingredient classification categories and shopping-list categories are separate.
- Ingredient data has no package information or ingredient-level nutrition.
- If recipe ingredient entries reference it, it must be archived rather than deleted. Existing entries remain valid, archived ingredients cannot be added to new entries, and permanent deletion is allowed once no entries reference it.

### Ingredient Classification Category

#### Ingredient Classification Category (Catalog Entity)

##### Purpose

A category used to classify ingredients independently of their shopping-list grouping.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the category | Yes | Unique |
| `Name` | Category name | Yes | Maximum length not specified |

##### Relationships

- May classify multiple Global or Private Ingredients; each ingredient may reference multiple classification categories.

##### Rules

- Ingredient classification categories are maintained by admins.
- Classification categories and shopping-list categories are separate concepts.

### Shopping-List Category

#### Shopping-List Category (Catalog Entity)

##### Purpose

A global category used to group ingredients on shopping lists.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the category | Yes | Unique |
| `Name` | Category name | Yes | Maximum length not specified |

##### Relationships

- May be assigned to multiple Global or Private Ingredients.
- May have a position in a user's personal ordering of the global categories.

##### Rules

- Shopping-list categories are maintained by admins; users cannot create custom categories.
- An ingredient without a preparation recipe must have a shopping-list category.
- Each user's category ordering is independent of other users' ordering.
- Shopping-list categories are distinct from ingredient classification categories.

#### Shopping-List Category Order (User Preference)

##### Purpose

The order in which one user prefers the global shopping-list categories to appear.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `User` | User whose preference this represents | Yes | References one User |
| `ShoppingListCategory` | Category being ordered | Yes | References one global Shopping-List Category |
| `Position` | Category's position in that user's ordering | Yes | Ordering value; exact representation not specified |

##### Relationships

- Belongs to one User and references one Shopping-List Category.

##### Rules

- A user's ordering does not change another user's ordering.

### Recipe Aggregate

#### Recipe (Aggregate Root)

##### Purpose

A user-owned recipe with instructions and ingredient amounts, a stated serving yield, and a total output amount.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the recipe | Yes | Unique |
| `Title` | Recipe name | Yes | Maximum 150 characters |
| `Description` | Additional recipe description | No | Maximum 1,500 characters |
| `ImageUrl` | Recipe image location | No | Maximum length not specified |
| `VideoUrl` | Recipe video location | No | Maximum length not specified |
| `Difficulty` | Recipe difficulty | Yes | `Easy`, `Medium`, `Advanced`, or `Expert` |
| `NumberOfServings` | Serving yield of the recipe | Yes | At least 1 |
| `OutputAmount` | Total physical amount produced, with its Unit | Yes | Amount is greater than 0 |
| `Nutrition` | Nutritional values for a stated amount and Unit | Yes | See Recipe Nutrition |

##### Relationships

- Owned by one User aggregate.
- Contains zero or more Recipe Ingredient entities and Recipe Sections.
- References Recipe Category aggregates.
- A Recipe Section contains ordered Recipe Steps.
- May be referenced as the preparation recipe by an Ingredient aggregate.

##### Rules

- Saved ingredient amounts make the recipe's stated number of servings.
- Preparation-recipe quantities scale by required ingredient amount divided by the preparation recipe's output amount.
- Recipe visibility, sharing, permissions, and independent copies are not part of this model.

#### Recipe Ingredient (Child Entity)

##### Purpose

An ingredient line in a recipe.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the recipe ingredient entry | Yes | Unique |
| `Ingredient` | Global or private ingredient used | Yes | References one ingredient |
| `Unit` | Unit used for the amount | Yes | References one Unit |
| `Amount` | Quantity of the ingredient | Yes | Numeric; range not specified |

##### Relationships

- Belongs to one Recipe aggregate.
- References one Global Ingredient or one Private Ingredient aggregate and one Unit.

##### Rules

- Archived ingredients remain valid in existing recipe entries but cannot be added to new entries.

#### Recipe Section (Value Object)

##### Purpose

A typed group of steps within a Recipe.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Type` | Section kind | Yes | `PreCooking`, `IngredientsPreparation`, `Cooking`, or `Serving` |

##### Relationships

- Owned by one Recipe aggregate.
- Contains zero or more Recipe Steps.

##### Rules

- Steps in a section have an order.

#### Recipe Step (Value Object)

##### Purpose

An ordered instruction within a Recipe Section.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Order` | Position of the step within its section | Yes | Integer |
| `Description` | Instruction text | Yes | Maximum 500 characters |
| `ImageUrl` | Optional image for the step | No | Maximum length not specified |

##### Relationships

- Owned by one Recipe Section value object.

##### Rules

#### Recipe Nutrition (Value Object)

##### Purpose

Nutritional values owned by a Recipe.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Calories` | Energy value | Yes | Integer |
| `Carbohydrates` | Carbohydrate value | Yes | Numeric |
| `Fats` | Fat value | Yes | Numeric |
| `Proteins` | Protein value | Yes | Numeric |
| `BasisAmount` | Amount for which the values are given | Yes | Integer |
| `BasisUnit` | Unit for the basis amount | Yes | References one Unit |

##### Relationships

- Owned by one Recipe aggregate.

##### Rules

- Nutritional values belong to Recipes, not Ingredients.

### Dishes Aggregate

#### Dish (Aggregate Root)

##### Purpose

A user-owned item that can be planned independently of a Recipe, with optional alternative preparation Recipes.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the Dish | Yes | Unique |
| `Name` | Display name of the Dish | Yes | Maximum length not specified |
| `ImageUrl` | Image for the Dish | Yes | URL constraints not specified |
| `Nutrition` | Nutritional values for one serving | Conditional | Optional when the Dish has associated Recipes; required and complete for a standalone Dish |

##### Relationships

- Owned by one User aggregate.
- May have zero or more associated Recipe aggregates; associated Recipes are alternative preparations, not components combined into the Dish.
- May have per-serving nutritional values; complete values are required when the Dish has no associated Recipes.
- May be referenced by Meal-Plan Entries.

##### Rules

- Dish nutrition describes one serving.
- A Dish with no associated Recipes must have nutrition values for calories, protein, fat, and carbohydrates.
- A Meal-Plan Entry for a Dish contributes one serving.
- When a planned Dish has associated Recipes, the user must select exactly one Recipe being prepared, and that Recipe's nutrition is used for the day's calculation. When it has no associated Recipes, the Dish's nutrition is used.
- If nutrition is unavailable for a planned item, mark daily nutrition totals incomplete rather than treating missing values as zero.

#### Dish Nutrition (Value Object)

##### Purpose

The nutritional values for one serving of a Dish.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Calories` | Energy value | Yes | Integer |
| `Carbohydrates` | Carbohydrate value | Yes | Numeric |
| `Fats` | Fat value | Yes | Numeric |
| `Proteins` | Protein value | Yes | Numeric |

##### Relationships

- Owned by one Dish aggregate.

### Recipe Category Aggregate

#### Recipe Category (Aggregate Root)

##### Purpose

A category used to classify recipes.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the category | Yes | Unique |
| `Name` | Category name | Yes | Maximum 100 characters |
| `Type` | Category classification | Yes | `Events`, `Cuisine`, `Course`, `MealType`, or `CookingMethod` |

##### Relationships

- May classify multiple Recipe aggregates; each Recipe may reference multiple Recipe Category aggregates.

##### Rules

- Recipe categories are maintained by admins.

### Cookbook Aggregate

#### Cookbook (Aggregate Root)

##### Purpose

A user's collection of recipes organized into categories.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the cookbook | Yes | Unique |
| `Title` | Cookbook name | Yes | Maximum 100 characters |
| `Description` | Cookbook description | Yes | Maximum 1,000 characters |
| `CoverImageUrl` | Optional cookbook cover image | No | Maximum length not specified |

##### Relationships

- Owned by one User aggregate.
- Contains zero or more Cookbook Category entities.

##### Rules

#### Cookbook Category (Child Entity)

##### Purpose

A named group of recipes within a Cookbook, optionally nested under another category.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the category | Yes | Unique |
| `Name` | Category name | Yes | Maximum 100 characters |
| `ParentCategory` | Optional parent category | No | References one Cookbook Category |

##### Relationships

- Belongs to one Cookbook aggregate.
- May contain subcategories and Recipes.

##### Rules

### Meal Plan Aggregate

#### Meal Plan (Aggregate Root)

##### Purpose

A user's plan for one calendar day.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the meal plan | Yes | Unique |
| `Date` | Calendar day of the plan | Yes | Date only; no time or timezone |

##### Relationships

- Owned by one User aggregate.
- Contains zero or more Meal-Plan Entries.

##### Rules

- Each user has exactly one Meal Plan per calendar date.

#### Meal-Plan Entry (Child Entity)

##### Purpose

One assignment of a Dish to a meal category in a Meal Plan, optionally identifying the preparation Recipe being used.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the entry | Yes | Unique |
| `Dish` | Assigned Dish | Yes | References one Dish |
| `Recipe` | Preparation Recipe being used | Conditional | Required when the Dish has associated Recipes; references one of them |
| `MealCategory` | Category for the assignment | Yes | References one Meal Category |

##### Relationships

- Belongs to one Meal Plan aggregate.

##### Rules

- Each entry represents one serving of its Dish.
- Multiple entries in one Meal Plan may use the same Meal Category.
- When the Dish has associated Recipes, the entry identifies the Recipe being prepared; the selected Recipe's nutrition is used for the day's calculation.
- When the Dish has no associated Recipes, the Dish's nutrition is used for the day's calculation.

### Meal Category Aggregate

#### Meal Category (Aggregate Root)

##### Purpose

A shared category for meal-plan entries, such as breakfast, lunch, dinner, or snack.

##### Fields

| Field | Meaning | Required | Constraints |
|---|---|---:|---|
| `Id` | Stable identity of the category | Yes | Unique |
| `Name` | Category name | Yes | Maximum length not specified |

##### Relationships

- May be used by multiple Meal-Plan Entries.

##### Rules

- Meal Categories are global and maintained by admins.

## Shopping-list derivation

A shopping list is a derived result generated on demand; it is not a persisted aggregate.

- A planned Dish with a selected Recipe contributes that Recipe's ingredients, scaled by planned servings divided by the Recipe's saved serving count. Each Dish entry represents one serving.
- If no Recipe is selected for a planned Dish, list the Dish under a dedicated "Dishes" category. Admins cannot manage this category.
- A list generated from directly selected Recipes uses their saved ingredient amounts without scaling.
- An ingredient with a preparation recipe is expanded into that recipe's ingredients. The amounts scale by the required prepared-ingredient amount divided by the preparation recipe's total output amount; expansion may continue through nested preparation recipes.
- When an applicable conversion is available, amounts normalize to the global ingredient's primary unit and the result includes information about the conversions used.
- When no applicable conversion is available, the original amount and unit are retained, the amount is marked unconverted, and it is not combined with amounts in other units.

## Identity domain

The identity account model and its email/password lifecycle are described in [wiki/domain/identity.md](../../wiki/domain/identity.md). This document references `User` as the owner of private ingredients, recipes, Dishes, cookbooks, meal plans, and category-order preferences; it does not redefine the identity model.

## Cross-aggregate references

Aggregate members reference other aggregate roots by identity; they do not contain those aggregates.

- Global and Private Ingredients reference Units, ingredient classification categories, and shopping-list categories. Private Ingredients may also reference a Global Ingredient; a preparation recipe is a Recipe reference.
- Shopping-list category ordering is a User preference referencing global Shopping-List Categories.
- Recipe Ingredients reference a Global or Private Ingredient and a Unit. Recipes reference their owner and Recipe Categories.
- Cookbook Categories reference Recipes.
- Dishes reference their owner and may reference alternative Recipes. A Dish has a required name and image URL.
- Meal Plans reference their owner; Meal-Plan Entries reference a Dish, exactly one selected Recipe when that Dish has associated Recipes, and a Meal Category.
- User and Unit group classifications are references rather than aggregates defined in this document. The catalog-entity boundaries for ingredient classification and shopping-list categories are not further specified.

## Open questions

- What are the maximum Dish name length and image URL constraints?
- Can one Recipe be associated with multiple Dishes?
- What numeric type, precision, and direction should `ConversionFactor` use?
- Should the reference unit's factor be explicitly fixed to `1`?
- What should happen when an admin attempts to delete a unit that is used by ingredients or recipes?
- What is the maximum nesting depth for Cookbook Categories?
