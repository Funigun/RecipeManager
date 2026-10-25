---
name: backend-feature-slice
description: Implement a backend vertical feature slice in RecipeManager.API using the Units feature conventions.
---

# Backend feature slice

Use this skill when adding or restructuring a backend feature in `RecipeManager.API/Features`. Follow the existing vertical-slice layout and reuse the API's `IAppDbContext`, `Result<T>`, endpoint mapping, authorization, and FluentValidation patterns.

## Feature layout

Keep each operation together under its domain folder. Follow the Units layout:

```text
Features/<Domain>/
  <Domain>Group.cs
  Command/<Operation>/
    <Operation>Endpoint.cs
    <Operation>Command.cs
    <Operation>Policies.cs        # when authorization or validation policies are needed
  Query/<Operation>/
    <Operation>Endpoint.cs
    <Operation>Query.cs
    <Operation>Policies.cs        # when authorization or validation policies are needed
```

Keep related request/response types and the handler in the command or query file unless the feature's size or existing domain conventions justify a separate file.

## Endpoint file

Declare `IEndpoint` implementations with `MapEndpoint` first, followed by a private static asynchronous handler:

```csharp
public sealed class GetThingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetThingResponse>(ThingsGroup.Route, Handle)
                 .WithName("GetThings")
                 .WithTags(ThingsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy");
    }

    private static async Task<Ok<GetThingResponse>> Handle(
        [FromServices] GetThingQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        GetThingResponse response = await queryHandler.Handle(cancellationToken);
        return TypedResults.Ok(response);
    }
}
```

Bind route/body inputs in the handler, wrap them in a command/query, call the handler, and translate `Result<T>` with the existing `ToHttpResult` helpers. Keep route metadata, authorization requirements, and documented problem responses consistent with the operation.

## Command/query file declaration order

Declare types in this order:

1. Request/input DTOs (including body DTOs, filters, or other inputs).
2. Command/query records that wrap the DTO and route/query parameters. Use a simple command/query with just the relevant identifiers when there is no body DTO.
3. Output/projection DTOs used internally.
4. Response DTOs and their mapping helpers.
5. Command/query handler.

For operations with no input, omit the unused DTO or wrapper rather than inventing one. Pass `CancellationToken` to asynchronous data-access and validation calls.

## Handler check and execution order

For a command that requires authorization and targets an existing entity, execute checks in this order:

1. Authorization; return `AuthorizationError` when denied.
2. Load the target entity; return `NotFoundError` when absent.
3. Validate the input; convert failures with `BaseValidator<T>.ToValidationError`.
4. Apply the change and persist only after all checks pass.

Create operations have no target-entity lookup, so authorize, validate, then create and persist. Delete operations generally authorize, load/check existence, then delete and persist. Queries should check existence before constructing a not-found response when the requested entity is absent.

Keep each check explicit in the handler; do not perform mutations or saves before the checks pass.

## Policy file

When policies are needed, declare the authorization policy before the FluentValidation validator. Keep authorization and validation as separate policies implementing `IAuthorizationPolicy<T>` and `IValidator<T>` (usually via `BaseValidator<T>`). 
Register/inject the policies using the existing feature dependency-injection conventions. The handler controls runtime ordering: authorization first, then any required not-found check, then validation.

## Verification checklist

- Confirm the endpoint maps the correct route, request/response type, authorization, and HTTP result.
- Confirm handler ordering is authorization → required not-found check → validation → operation.
- Confirm policy classes are ordered authorization before validation and validate the correct input.
- Build the API and run the narrowest relevant feature tests.
