# Recipe Manager

A comprehensive application for managing user ingredients, recipes, cooking books, and meal plans. Built with .NET 10, .NET Aspire, and Blazor, adhering to Vertical Slice Architecture and REPR patterns.

## Overview
The Recipe Manager allows culinary enthusiasts to digitize their kitchen. It supports two main user roles:

*   **Regular Users**: Manage personal ingredients, recipes, cooking books, and meal plans.
*   **Admin Users**: Include all regular capabilities plus management of global data like measurement units, recipe categories, and ingredient categories.

---

## Features overview:
The application is organized by clear domains:

*   **Recipes**: Create detailed culinary instructions with steps, ingredient links, and difficulty ratings.
*   **Ingredients**: Manage food items including nutritional values, package sizes, and shopping lists.
*   **Cookbooks**: Organize recipes into a personal hierarchy (Cookbook -> Category -> Subcategory).
*   **Meal Planner**: Schedule recipes for specific dates and meals.
*   **Units**: Manage measurement units (e.g., Grams, Cups) and conversions.
*   **Categories**: Classification systems for both Ingredients (e.g., Vegetables, Dairy) and Recipes (e.g., Breakfast, Italian).

---

## Architecture & Tech Stack

This project is built on the strict standards of **.NET 10** using the following architecture and technologies:

### Core Frameworks
*   **.NET Aspire**: Orchestration for local development and cloud deployment.
*   **.NET Web API**: Backend services using Minimal APIs.
*   **Blazor**: Hybrid frontend using Blazor Server and WebAssembly.
*   **Entity Framework Core**: Code-first data access with `IAppDbContext` abstraction.
*   **Github Copilot**: AI-assisted development for code generation and refactoring.

### Solution Structure
*   **Aspire Orchestration**: Manages startup and inter-service discovery.
*   **Backend APIs**:
    *   `Recipe API`: Core domain logic (Units, Ingredients, Recipes, Plans).
    *   `Identity API`: User management and authentication.
    *   `API Shared`: Common HATEOAS services, DTOs, and filtering logic.
*   **Frontend**: Unified Blazor application.
*   **Shared Contracts**: FluentValidation validators shared between backend and frontend.

### Patterns
*   **Vertical Slice Architecture**: Features are self-contained slices rather than horizontal layers.
*   **REPR Pattern**: Request-Endpoint-Response pattern for API interactions.
*   **HATEOAS**: Hypermedia as the Engine of Application State implemented via builder pattern.
*   **Caching**: Hybrid caching strategy using Redis and in-memory caches with Decorator pattern combined with Repository pattern.

## Quality Standards

This project enforces strict quality gates:

*   **Coding Standards**: Enforced via global `.editorconfig` and `Directory.Build.props`.
*   **Static Analysis**: Uses `StyleCop.Analyzers` and `SonarAnalyzer.CSharp`.
*   **Testing**:
    *   **Unit Tests**: xUnit with mocks (focus on isolation).
    *   **Integration Tests**: xUnit with **Testcontainers** (Db/Redis) for real-world scenarios.
    *   **Architecture Tests**: Enforced architecture desisions using `NetArchRules`.

## Documentation

*   **ADR**: Architectural Decision Records are located in `wiki/adr`.
*   **Domain Models**: Detailed explanations of the domain are in `wiki/domain`.

---

## Project History & Key Points
This project was created as part of a "WEB API Masters" course from Szkola Dotneta platform.

- **September 2024**: Project started, following the course with controllers, clean architecture, CQRS, and the MediatR library.
- **May 2025**: Refactored to Minimal API with Vertical Slice architecture; removed MediatR.
- **July 2025**: Began work on Blazor UI (Blazor was mentioned in the course, but the web API was the core topic), using MS 365 and GitHub Copilot.
- **January 2026**: Started configuring GitHub Copilot and used it to refactor parts of the application; generated some ADR files with Copilot's help.

---

## Most important skills aquired
- Understanding of REST API design principles
- Understanding of Clean Architecture and Vertical Slice Architecture
- Experience with Controllers and Minimal APIs
- Configure and use Entity Framework with MS SQL
- Implement HATEOAS with builder pattern
- Implement caching strategies with Redis and Hybrid Cache with Decorator pattern
- Implement unit tests using xUnit
- Implement integration tests using xUnit and Respawn
- Implement architecture tests using xUnit and .NetArchRules
- Implement Authentication and Authorization using JWT Bearer tokens using .Net built-in features as separate API
- Build Blazor UI application
- Group everything using Aspire framework
- Configure observability using Open Telemetry, Serilog, and Aspire dashboard
- Use GitHub Copilot to speed up development and improve code quality