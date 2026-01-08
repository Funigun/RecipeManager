# 3. Blazor for Frontend and MudBlazor Component Library

Date: 2025-07-01
Status: Accepted

## Context
The solution requires a rich, interactive user interface for managing recipes and meal plans. The team possesses strong C# and .NET skills. Introducing a JavaScript framework (React/Angular) would introduce context switching, duplicate validation logic, and require separate build pipelines.

## Decision
We decided to use **Blazor** (targeting .NET 10) for the frontend application.
We selected **MudBlazor** as the UI component library.

- **Hosting Model**: Blazor Server + WebAssembly (Hybrid/Auto) to optimize for both initial load time and offline/interactive capabilities.
- **UI Components**: MudBlazor provides Material Design components out-of-the-box, speeding up development.
- **Shared Contracts**: Logic for DTOs and Validators (FluentValidation) is shared between the Backend API and Blazor Frontend via a shared class library.

## Consequences
### Positive
- **Language Uniformity**: C# used across the full stack.
- **Code Reuse**: Validation rules and DTOs are shared, ensuring consistency between client and server.
- **Productivity**: Rapid UI development using pre-built MudBlazor components specifically designed for Blazor.

### Negative
- **Payload Size**: The WebAssembly download size is larger than typical JS frameworks (mitigated by Blazor Server rendering on first load).
- **DOM Control**: MudBlazor abstracts the HTML/CSS; heavy customization requires understanding the library's internal rendering.
