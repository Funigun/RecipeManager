# 6. Use Builder Pattern for HATEOAS Response Construction

Date: 2024-05-23
Status: Accepted

## Context

The RecipeManager application uses HATEOAS (Hypermedia as the Engine of Application State) to provide navigational links in API responses. This allows clients to dynamically discover available actions and transitions.

Constructing these responses manually within endpoints or services can be repetitive, error-prone, and verbose. It involves:
1.  Checking user permissions for specific actions.
2.  Generating URLs based on endpoint definitions.
3.  Creating `Link` objects.
4.  Assembling `HateoasResponse<T>` or `HateoasCollectionResponse<T>` wrappers.

We need a standardized, reusable, and fluent mechanism to construct these responses to ensure consistent API behavior and simplify development.

## Decision

We will implement a **Builder Pattern** to handle the construction of HATEOAS responses.

This involves the following key components in the `RecipeManager.Api.Shared` project:

1.  **`IHateoasBuilderFactory` / `HateoasBuilder`**: The entry point for creating response builders. It injects necessary services (like `HateoasLinkService`).
2.  **`HateoasResponseBuilder<TItem>`**: Specializes in building responses for single resources. It allows adding method-specific links (GET, POST, PUT, DELETE) and pagination links if applicable.
3.  **`HateoasCollectionResponseBuilder<TItem>`**: Specializes in building responses for collections of resources. It handles links for the collection itself.
4.  **`HateoasCollectionListBuilder<TItem>`**: An extension of the collection builder that iterates over the items in the collection to add links to each individual item.

## Technical Details

### Class Responsibilities

*   **`HateoasBuilder`**: Implements `IHateoasBuilderFactory`. Exposes `ForItem<T>(T item)` and `ForCollection<T>(IEnumerable<T> items)` methods.
*   **`HateoasResponseBuilder<TItem>`**:
    *   Maintains the item and a list of links.
    *   Provides fluent methods: `AddGet`, `AddPost`, `AddPut`, `AddDelete`, `AddPagedNavigation`.
    *   Checks `LinkOptions.IsActionAllowed` before adding a link.
    *   Uses `HateoasLinkService` to generate the actual URL.
*   **`HateoasCollectionResponseBuilder<TItem>`**:
    *   Wraps a list of items into `HateoasResponse<TItem>`.
    *   Provides methods to add links to the *collection root* (e.g., "create new item").
    *   Exposes `WithCollectionLink()` to transition to modifying individual item links.
*   **`HateoasCollectionListBuilder<TItem>`**:
    *   Inherits from `HateoasCollectionResponseBuilder`.
    *   Provides `WithGet`, `WithPut`, `WithDelete` methods that accept a delegate `Func<TItem, object>` to extract route parameters from each item.
    *   Iterates through the collection and invokes the link generation for each item.

### Usage Pattern

The builders allow for a fluent chaining of operations.

**Single Item Example:**
```csharp
var response = _hateoasBuilder.ForItem(recipe)
    .AddGet(LinkOptions.Create(nameof(GetRecipeById), "self"), new { id = recipe.Id })
    .AddPut(LinkOptions.Create(nameof(UpdateRecipe), "update"), new { id = recipe.Id })
    .AddDelete(LinkOptions.Create(nameof(DeleteRecipe), "delete"), new { id = recipe.Id })
    .Build();
```

**Collection Example:**
```csharp
var response = _hateoasBuilder.ForCollection(recipes)
    .AddGet(LinkOptions.Create(nameof(GetRecipes), "self"), null)
    .AddPost(LinkOptions.Create(nameof(CreateRecipe), "create"), null)
    .WithCollectionLink() // Transition to item-level links
        .WithGet(LinkOptions.Create(nameof(GetRecipeById), "self"), x => new { id = x.Id })
        .WithPut(LinkOptions.Create(nameof(UpdateRecipe), "update"), x => new { id = x.Id })
    .Build();
```

## Consequences

### Positive
*   **Consistency**: All HATEOAS responses follow the same structure and validation logic.
*   **Readability**: The fluent API makes the intent of the code clear in the endpoint handlers.
*   **Encapsulation**: The complex logic of URL generation and permission checking is hidden from the consumer.
*   **Maintainability**: Changes to the HATEOAS structure need only be made in the builder classes.

### Negative
*   **Complexity**: Introduces a layer of abstraction that new developers must learn (though the pattern is standard).
*   **Performance**: Slight overhead due to object allocation for builders and delegates, though negligible for standard API throughput.
