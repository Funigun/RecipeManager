# 7. Endpoint Abstractions and Extensions

Date: 2024-05-23
Status: Accepted

## Context

As the application grows, managing a large number of Minimal API endpoints can become challenging. Without a structured approach, we face several issues:
1.  **Inconsistent Registration**: Endpoints might be registered in the main `Program.cs`, leading to a massive, unmaintainable file.
2.  **Repetitive Boilerplate**: Applying common logic like validation, authentication, and standard error responses (400, 404, 500) to every endpoint results in duplicate code.
3.  **Lack of Organization**: Implicit grouping of related endpoints (e.g., all "Recipe" endpoints) is often missing or implemented ad-hoc.
4.  **Discovery**: Manually registering every new endpoint is error-prone; forgetting one leads to runtime 404s.

We need a system to modularize, govern, and automatically discover endpoints.
We decided to implement set of abstractions and extensions in the same way as Kajetan Duszynski shoved in his demo project:
- [LinkedIn post](https://www.linkedin.com/posts/kduszynski_github-szkoladotnetaminimalapi20-vsa-activity-7345337843071737856-CEjg/?utm_source=share&utm_medium=member_desktop&rcm=ACoAAC4QGP4Bg2ocN1fTIIM4nTBhPHpRysnXTDY)]
- [Repository](https://github.com/szkoladotneta/minimalapi2.0-vsa)]

## Decision

We will implement a set of abstractions and extensions in `RecipeManager.Api.Shared` to standardize endpoint development.

### 1. Endpoint Interfaces
*   **`IEndpoint`**: Defines a contract for a class that registers a single route or a small set of closely related routes. It requires a `MapEndpoint` method.
*   **`IGroupEndpoint`**: Defines a contract for configuring a route group (e.g., applying tags, prefixes, or shared middleware). It works in conjunction with route grouping.

### 2. Grouping Mechanism
*   **`GroupEndpointAttribute`**: An attribute to declarative associate an `IEndpoint` implementation with a specific group name (e.g., `[GroupEndpoint("recipes")]`).
*   **Automatic Grouping**: During startup, endpoints carrying this attribute are automatically nested under the corresponding `RouteGroupBuilder` managed by the matching `IGroupEndpoint`.

### 3. Automatic Registration
*   **Scanning**: `AddEndpoints(Assembly assembly)` scans the provided assembly for all implementations of `IEndpoint` and `IGroupEndpoint` and registers them in the DI container.
*   **Mapping**: `MapEndpoints(WebApplication app)` retrieves all registered endpoints, groups them based on the attribute, applies the group configuration, and then maps the individual routes. Default route prefixes (e.g., `api/{groupName}`) are applied automatically.

### 4. Fluent extensions
We introduce `EndpointMappingExtensions` to reduce boilerplate for common scenarios. These extensions wrap standard Minimal API builders to:
*   Automatically declare `ProducesProblem` for standard error codes (400, 404, 500).
*   Add metadata for OpenAPI documentation.
*   Integrate validation and authentication filters seamlessly.

**Key Extensions:**
*   `MapStandardGet<TResponse>` / `MapStandardAuthenticatedGet<TResponse>`
*   `MapStandardPost<TRequest, TResponse>` / `MapStandardAuthenticatedPost<TRequest, TResponse>`
*   `MapStandardPut<TRequest>` / `MapStandardAuthenticatedPut<TRequest, TDto>`
*   `MapStandardDelete<TRequest>`

## Usage Example

**Defining a Group:**
```csharp
public class RecipeGroup : IGroupEndpoint
{
    public string GroupName => "recipes";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithTags("Recipes");
    }
}
```

**Defining an Endpoint:**
```csharp
[GroupEndpoint("recipes")]
public class CreateRecipe : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardAuthenticatedPost<CreateRecipeRequest, CreateRecipeResponse>(
            "/", 
            Handler)
            .WithSummary("Create a new recipe");
    }

    private static async Task<IResult> Handler(/* dependencies */) { ... }
}
```

## Consequences

### Positive
*   **Modular Architecture**: Each endpoint is a self-contained class, adhering to the Single Responsibility Principle (part of REPR pattern).
*   **Consistency**: All endpoints automatically document standard error responses and follow the same configuration patterns.
*   **Developer Experience**: Adding a new endpoint is as simple as creating a class and adding an attribute; no changes needed in `Program.cs`.
*   **Clean `Program.cs`**: The entry point remains concise and focused on infrastructure setup.

### Negative
*   **Discovery Magic**: New developers might not immediately see where routes are registered if they are unfamiliar with the scanning convention.
*   **Reflection Overhead**: Startup time is slightly increased due to assembly scanning, though this is generally negligible for standard applications.
