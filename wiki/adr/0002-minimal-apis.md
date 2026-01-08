# 2. Use of .NET Minimal APIs

Date: 2025-05-01
Status: Accepted

## Context
The project was originally built using ASP.NET Core Controllers. As the move to Vertical Slice Architecture was planned, Controllers were seen as adding unnecessary boiler-plate (inheritance, attribute rooting) that didn't align well with the single-responsibility nature of vertical slices. We aimed to utilize .NET 10 features for more concise code.

## Decision
We decided to implement all HTTP endpoints using **ASP.NET Core Minimal APIs**.

- Endpoints are defined using extension methods (e.g., `app.MapGet`, `app.MapPost`).
- Endpoint definitions are grouped by feature slice.
- Filters are used for cross-cutting concerns like validation and error handling.

## Consequences
### Positive
- **Performance**: Theoretically lower overhead than MVC Controllers.
- **Conciseness**: Significant reduction in boilerplate code.
- **Alignment**: Fits naturally with the Vertical Slice/REPR pattern where one file/class often represents one endpoint.

### Negative
- **Discoverability**: Unlike controllers, endpoints aren't automatically grouped by class name, requiring discipline in organizing extension methods for route mapping.
