# 1. Adoption of Vertical Slice Architecture

Date: 2025-05-01
Status: Accepted

## Context
The project initially followed a traditional Clean Architecture approach with CQRS and MediatR (layers: Domain, Application, Infrastructure, API). While this provided separation of concerns, it resulted in low cohesion where logic for a single feature was scattered across multiple projects and folders. Simple changes required navigating through multiple layers, and the abstraction overhead of MediatR was deemed unnecessary for the complexity of the domain.

## Decision
We decided to refactor the application to use **Vertical Slice Architecture**, specifically implementing the REPR (Request-Endpoint-Response) pattern.

- Features are organized by domain slices (e.g., `Features/Recipes/Create`, `Features/Recipes/Get`).
- Each slice contains the API endpoint, request command/query, validation logic, and handler logic in close proximity.
- Shared kernel/infrastructure code is minimized and kept separate from feature logic.
- MediatR was removed in favor of direct invocation or lightweight handlers within Minimal API endpoints.

## Consequences
### Positive
- **High Cohesion**: All code related to a specific feature lies in one place.
- **Maintainability**: Changing a feature involves touching only one slice, reducing side effects.
- **Flexibility**: Different slices can use different patterns (e.g., complex logic for Meal Plans, simple CRUD for Units) without enforcing a uniform layered structure.

### Negative
- **Code Duplication**: There is a potential for duplicating logic between slices, though this is often acceptable to decouple features.
- **Learning Curve**: Developers accustomed to layered architecture may need time to adapt to the structure.
