# 4. Cookbook and CookbookCategory Domain Models explanation

Date: 2024-05-23
Status: Accepted

## Context

The `Cookbook` and `CookbookCategory` domain models provide a way for users to organize their recipes into structured collections, mimicking real-world cookbooks. This supports use cases like "My Favorite Italian Recipes" or "Weekly Meal Prep".

### Cookbook
The `Cookbook` model represents the container. It acts as the root entity for a user's collection.
- **Metadata**: `Title`, `Description`, and `CoverImageUrl` allows for rich presentation in the UI.
- **Structure**: It strictly owns its categories; a cookbook is composed of categories.

### CookbookCategory
The `CookbookCategory` model allows for a hierarchical organization within a cookbook (e.g., Cookbook "Dinner Ideas" -> Category "Pasta" -> Subcategory "Vegetarian").
- **Hierarchy**: Supports infinite nesting via a self-referencing `Subcategories` list and a `ParentId`.
- **Content**: It acts as the leaf node that actually holds references to `Recipes`. A cookbook doesn't hold recipes directly; they are organized into categories (even if it's just a default "General" category).

For more details on operations, see `Features/Cookbooks`.

## Relationships

- **Cookbook to CookbookCategory**:
    - A One-to-Many relationship. A `Cookbook` fully owns its `Categories`.
    - Configured in `CookbookConfiguration`, utilizing `HasMany(...).WithOne(...)`.
- **CookbookCategory to CookbookCategory (Hierarchy)**:
    - A self-referencing One-to-Many relationship defined by `ParentId`.
    - Configured in `CookbookCategoryConfiguration` with `DeleteBehavior.NoAction` to prevent cyclic cascade issues.
- **CookbookCategory to Recipe**:
    - A Many-to-Many conceptual relationship, implemented as a collection of `RecipeId` value objects.
    - Configured via `OwnsMany` in `CookbookCategoryConfiguration`, mapping to the joining table `CookbookCategoryToRecipe`. This allows a single recipe to be placed in multiple cookbook categories across different cookbooks.
