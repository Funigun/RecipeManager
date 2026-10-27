## Domain analysis

The main change is to **separate shared catalog data from user-owned data**, rather than turning today's personal `Ingredient` and `Recipe` aggregates into globally writable records.

Today, `Ingredient` combines catalog-like facts—name and nutrition—with personal details such as package size, unit conversions, and shopping-list grouping (`RecipeManager.API/Domain/Ingredients/Ingredient.cs`). Recipes refer directly to those ingredients, while cookbooks and meal plans are separate aggregates that hold recipe IDs. Ownership is inferred from `AuditableEntity.CreatedBy`; the model has no explicit owner or sharing relationship (`RecipeManager.API/Domain/Common/Abstractions/AuditableEntity.cs`, `RecipeManager.API/Persistance/AppDbContext.cs`). Units and ingredient/recipe categories are already shared reference data managed by admins, so they provide an existing precedent for global catalogs.

A future domain would need to distinguish:

- **Canonical products/ingredients**: shared identity and reusable facts, such as a product name and baseline nutrition.
- **Personal ingredient records**: user-specific package, conversion, shopping-list, or other household details, linked to a canonical item where appropriate.
- **Authored recipes**: creator/owner and recipe content, with an explicit access concept separate from audit fields. Sharing with selected people or a family group is different from publishing globally.
- **Recipe ingredient references**: a clear rule for whether a recipe references canonical products, personal ingredient records, or both. Current direct links to personal ingredients can make a recipe hard to reuse or share.
- **Personal organization**: cookbooks and meal plans should remain user-owned unless shared-cookbook or shared-planning use cases are deliberately introduced. Sharing a recipe should not implicitly share its cookbook or meal plan.

There is also a content-lifecycle decision to make: if a shared recipe or product changes, should recipients see the latest version, a fixed shared version, or a copy they can adapt? The current model has no version or snapshot concept, so this should be decided before relying on shared records.

**Important current-state caveat:** recipe list and detail queries do not consistently restrict reads to the creator, while write authorization does (`GetRecipesQuery.cs`, `GetRecipeByIdQuery.cs`, `UpdateRecipePolicies.cs`). That is not a defined sharing model; it is an inconsistent boundary to account for when establishing explicit access rules.

This analysis is limited to domain-model changes; it does not assess feature behavior, endpoints, or UI.
