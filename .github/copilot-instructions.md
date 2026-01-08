# Github Copilot instructions for Reipe Manager project

## Project overview:

This project is an application for managing user ingredients, recipes, cooking books and meal plans.

There are two main user roles:
- Regular Users: Can manage their own ingredients, recipes, cooking books, and meal plans
- Admin Users: Have all the capabilities of regular users, plus the ability to manage measurement units, recipe categories and ingredient categories

## Architecture & Patterns:
- **Framework**: Aspire Orchestration, .Net Web API (.Net 10 + Minimal APIs), Blazor Server + WebAssembly
- **Pattern**: Vertical Slice Architecture with REPR pattern
- **Data Access**: Entity Framework Core with code-first approach - 'IAppDbContext' abstraction for DbContext
Whole application is built in .Net 10 with following components:

## Solution structure:
- Aspire orchestration
- Backend APIs: 
	- Recipe API: measurement units, ingredient categories, ingredients, recipe categories, recipes, cooking books, meal plans
	- Identity API: user management and authentication
	- API Shared: common code shared between APIs e.g. Hateoas services and DTOs, filtering interfaces, Minimal API endpoint interfaces and extensions
- Frontend: Blazor Server + WebAssembly application
- Shared Contracts: things common in both backend and frontend, at this moment there are Validators based on FluentValidation

For more details refer to:


# Coding standards

Project uses global files for coding standards and practices:
- .editorconfig
- Directory.Build.props
- Directory.Packages.props

In case you need to add new package references, analyzers or change coding styles, please do so in these global files.

There are also nuget packages for code analysis and style enforcement:
- StyleCop.Analyzers
- SonarAnalyzer.CSharp
