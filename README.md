# RecipeManager

## Overview
RecipeManager is an application where users can manage cooking recipes, ingredients, cooking books, and meal plans. It also allows users to generate shopping lists for their planned meals.
Recipes and ingredients have nutrition facts so users can plan their meals according to their dietary needs.
Administrators are responsible for managing measurement units, ingredient categories, and recipe categories.

Features overview:
-  

---

## Project History & Key Points
This project was created as part of a "WEB API Masters" course from Szkola Dotneta platform.

- **September 2024**: Project started, following the course with controllers, clean architecture, CQRS, and the MediatR library.
- **May 2025**: Refactored to Minimal API with Vertical Slice architecture; removed MediatR.
- **July 2025**: Began work on Blazor UI (Blazor was mentioned in the course, but the web API was the core topic), using MS 365 and GitHub Copilot.
- **January 2026**: Started configuring GitHub Copilot and used it to refactor parts of the application; generated some ADR files with Copilot's help.

---

## Tech Stack
- **Framework**: .NET 10 Web API, Blazor, Aspire
- **Database**: MS SQL with Entity Framework
- **Cache**: Redis + Hybrid Cache
- **Monitoring**: Aspire dashboard, Serilog, Redis

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