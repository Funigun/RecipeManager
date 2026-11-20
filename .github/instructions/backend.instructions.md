---
description: 'Instructions for the RecipeManager backend API feature architecture'
applyTo: '**/RecipeManager.API/Features/**/*.cs'
---

# Backend API Instructions

## 1) Application direction

- Follow the existing .NET Minimal API and vertical-slice architecture. Do not introduce a different hosting, mediator, or persistence pattern unless explicitly requested.
- Keep HTTP concerns in endpoints, use cases in command/query handlers, and business rules in the domain model. Do not assume a particular feature, helper, or shared type exists; inspect nearby slices and reuse established patterns.
- Use `IAppDbContext` for persistence rather than depending on the concrete database context.

## 2) Vertical slices

- Organize backend work by domain and operation under `Features/<Domain>/Command/<Operation>/` or `Features/<Domain>/Query/<Operation>/`.
- Keep an operation's endpoint, request/command or query, response, handler, and applicable policies together. Keep small related DTOs and mappings in the operation file; split them only when their size or reuse justifies it.
- Put shared route constants and route builders in the domain's group class. Reuse them rather than duplicating route strings.
- Keep each handler focused on one use case. Do not move unrelated operations into a catch-all service or handler.

```text
Features/
└── <Domain>/
    ├── <Domain>Group.cs
    ├── Command/
    │   └── <Operation>/
    │       ├── <Operation>Endpoint.cs
    │       ├── <Operation>Command.cs
    │       └── <Operation>Policies.cs       # when needed
    └── Query/
        └── <Operation>/
            ├── <Operation>Endpoint.cs
            ├── <Operation>Query.cs
            └── <Operation>Policies.cs        # when needed
```

## 3) Endpoints and HTTP contracts

- Implement endpoints with `IEndpoint`. Map them through the existing `MapStandardGet`, `MapStandardPost`, `MapStandardPut`, or `MapStandardDelete` helpers.
- Keep endpoint handlers thin: bind route, query, and body inputs; construct the command/query; call its handler; translate the outcome into a typed HTTP result.
- Include the domain group's route and tag, a stable endpoint name, and a useful description. Declare relevant success and problem responses and apply the established authentication policy.
- Use `ToHttpResult` for `Result<T>` outcomes. Return a direct typed result when the handler has no error outcome to translate. Choose status codes and resource locations to match the operation and existing API conventions.
- Keep route templates and resource URL builders in the domain group; avoid deriving URLs ad hoc in handlers.

## 4) Commands, queries, and handlers

- Define transport request DTOs separately from commands/queries when that clarifies the API boundary. Wrap request data and route identifiers in the operation's command/query; omit redundant wrappers when the established pattern for that operation does not need one.
- Map request DTOs to domain types at the handler boundary, preferably using focused mapping methods. Use domain factories and methods for creation and mutation rather than bypassing domain behavior.
- Inject only the operation's dependencies, normally `IAppDbContext` plus validators and authorization policies where needed.
- Pass the request `CancellationToken` to asynchronous validation and data-access operations.
- For reads, use EF Core async queries, apply user/ownership filters in the query, use `AsNoTracking()` for read-only projections, and project only the data the response needs. Reuse the established paging and sorting helpers where applicable.
- Return `NotFoundError` or other established result errors for expected failure cases. Perform existence and ownership checks before mutating; do not save changes until authorization and validation have passed.
- Persist command changes through the injected context and `SaveChangesAsync`. Queries must not mutate or save.

## 5) Validation and authorization

- Implement request validation with FluentValidation. Derive feature validators from `BaseValidator<T>` where applicable and reuse shared validation rules from `Features/Validation`.
- Keep validation policies near the operation. Inject `IValidator<TRequest>` into the handler and translate invalid results with `BaseValidator<TRequest>.ToValidationError`.
- Use `IAuthorizationPolicy<T>` for operation-specific role or resource authorization. Check authorization before revealing resource existence when the operation's security rules require it; otherwise follow the established order for that use case.
- Keep authentication requirements on the endpoint and domain authorization in the handler/policy. Do not rely on UI visibility or request-supplied ownership data as authorization.
- Preserve operation-specific order: generally authorize, check target existence/ownership when required, validate, then mutate and save. Queries should scope results to the current user where the feature is user-owned.

## 6) Registration and discovery

- Endpoints are discovered by the existing assembly-based `MapEndpoints` mechanism; do not add manual endpoint mappings.
- Register each new command/query handler as a scoped service in the API's existing dependency-registration location. Validators and authorization policies use the existing assembly registration extensions.
- Follow existing domain group and policy naming conventions, and update any directly dependent API contract or test when the endpoint behavior changes.

## 7) Verification

- Confirm route, HTTP verb, request/response contract, authorization, status codes, and documented problem responses.
- Confirm handlers pass cancellation tokens, enforce authorization/ownership and validation before writes, and use the domain model and `IAppDbContext`.
- Build the API and run the narrowest relevant backend tests for the changed feature.
