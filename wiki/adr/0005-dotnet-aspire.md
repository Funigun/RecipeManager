# 5. Application Orchestration with .NET Aspire

Date: 2026-01-01
Status: Accepted

## Context
The solution has grown to include multiple components: a Recipe API, an Identity API, a Blazor Frontend, a SQL Database, and a Redis Cache. Managing the local development environment (connection strings, ports, startup order) and observability was becoming complex.

## Decision
We decided to adopt **.NET Aspire** for orchestration.

- An `AppHost` project manages the lifecycle of all services and containers.
- Service discovery is handled automatically via injected environment variables.
- The Aspire Dashboard is used for centralized logging, metrics, and distributed traces.

## Consequences
### Positive
- **Developer Experience**: "F5" experience to start the entire distributed system.
- **Observability**: OpenTelemetry is configured out-of-the-box with a unified dashboard.
- **Configuration**: Removed fragile hardcoded ports and connection strings from `appsettings.json`.

### Negative
- **Dependency**: Adds a dependency on the Aspire workload and runtime.
