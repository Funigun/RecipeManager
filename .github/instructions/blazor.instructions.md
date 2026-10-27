---
description: 'Instructions for the Blazor UI architecture'
applyTo: '**/RecipeManager.UI/**/*.razor,**/RecipeManager.UI/**/*.cs,**/RecipeManager.UI.Blazor/**/*.razor,**/RecipeManager.UI.Blazor/**/*.cs'
---

# Blazor UI Instructions

## 1) Application direction

- Follow the Blazor hosting and rendering model chosen for the UI; do not introduce a hosting-model migration unless explicitly requested.
- Keep server-only concerns out of reusable UI components and any client-side assemblies.
- Do not assume that a particular UI project, feature, component, service, or shared infrastructure already exists. Follow established repository conventions where available; otherwise, use the architecture below as the default.

## 2) Vertical Slice Architecture

- Organize UI by module, aggregate, and user action. Use the existing domain areas (such as Recipes, Ingredients, Cookbooks, Meal Planner, Units, and Categories) where they fit; do not create duplicate UI modules solely to mirror API project boundaries.
- Keep routes and slice roots action-specific. Do not combine Create, Read, and Edit API orchestration in one page or slice root selected by a mode parameter.
- When Create, Read, and Edit intentionally use the same fields and validation, use one shared aggregate form. It may accept an explicit mode for presentation only; the owning slice root supplies its model and callbacks and remains responsible for API calls, authorization, navigation, and choosing the operation. In Read mode, render fields read-only and omit submit actions.
- Pages define the route, layout, authorization, localized page title, and render the slice root. Keep workflow markup out of pages.
- A slice root owns its workflow: loading data, API calls, validation, error handling, and user interactions. It can be split into sub-components for visual or responsibility separation.
- Prefer action-oriented slices such as `List{Aggregate}`, `View{Aggregate}`, `Create{Aggregate}`, `Edit{Aggregate}`, and `Delete{Aggregate}` when those actions apply.
- Each slice owns its route, page state, API orchestration, validation, loading state, error handling, authorization, and post-action navigation.
- Share visual field groups, display components, and forms when their fields and validation are intentionally common. Do not share slice roots or move workflow orchestration into a form.
- A shared mode form belongs in the aggregate root `Components/` and uses an aggregate-level form model and validator in `Models/`. Keep separate action-specific forms and models when workflows have materially different fields or validation.
- Shared components receive data and callbacks. They do not load data, call APIs, perform navigation, or decide which operation is executing.
- Keep workflow roots beside their page within the feature slice. Use `Shared/` only for reusable components, models, contracts, clients, and cross-slice UI primitives.
- Define dependency-injection registration per module when a module has services to register.

### Slice structure

Use this as a naming and organization reference, not a requirement to create every file or folder for every slice:

```text
Features/
└── {Module}/
    ├── DependencyInjection.cs
    ├── Shared/                              ← module-wide UI primitives, if needed
    └── {Aggregate}/
        ├── {Aggregate}Page.razor            ← aggregate entry page, if applicable
        ├── {Aggregate}View.razor             ← entry slice root, if applicable
        ├── Components/                      ← entry slice parts, if needed
        ├── Models/                          ← entry slice models, if needed
        ├── Shared/                          ← clients, contracts, routes, if shared
        ├── Create{Aggregate}/
        │   ├── Create{Aggregate}Page.razor
        │   ├── Create{Aggregate}View.razor
        │   ├── Components/
        │   └── Models/
        └── Edit{Aggregate}/
            ├── Edit{Aggregate}Page.razor
            ├── Edit{Aggregate}View.razor
            ├── Components/
            └── Models/
```

- Folder name = slice name. Avoid generic action-slice folders such as `Management`, `Pages`, or `Models` at aggregate level.
- An aggregate entry page is not necessarily a list. Aggregates made of independent workflows need not have an aggregate-root page.
- Only routable components end with `Page`; other components are slice roots or parts of a slice.
- A slice with one workflow needs no subfolders other than those it uses, such as `Components/` and `Models/`.
- Place resources beside the slice when they are slice-specific; place resources in the aggregate root when multiple slices share them.
- Name action-specific forms and models for their action and aggregate. For an intentionally shared mode form, use aggregate-level names.
- Slices that cannot be represented as one action may be named after their main view or workflow.
- Mixed list-and-dialog workflows keep the dialog in `Components/`; the slice root owns API calls and passes data and callbacks to the dialog.

### API result and error handling

- Keep HTTP transport and response interpretation in API clients or shared HTTP infrastructure, not in presentation-only components.
- Give slice roots a consistent way to handle loading, validation, request failures, and unexpected errors. Follow existing shared infrastructure if present; do not assume project-specific result or error-state types.
- Keep load errors separate from submit errors so a failed submission does not discard loaded content or form state.
- Localize user-facing errors. Do not expose raw exception details or server diagnostics to users.
- Pass validation errors and other display state into forms as parameters; forms should not make API requests to discover errors.
- Ensure submitting state is reset on both success and failure.
- Map API contracts to UI-specific view models where the UI needs a distinct presentation model; map form models to request contracts at the workflow boundary.

## 3) API clients

- Keep UI API clients focused on HTTP communication and resource operations; avoid one client that becomes a catch-all for an entire module.
- A resource client may be used by multiple UI slices.
- Keep UI code independent of backend implementation and domain layers.
- Avoid coupling UI components directly to API transport contracts. Use UI-specific view models and map to or from API contracts at a clear boundary.
- Keep cross-resource orchestration in the owning slice root or a dedicated service when it is genuinely shared.
- Group typed HTTP client registrations by module through dependency-injection extensions when typed clients are used.

## 4) Routes and navigation

- Declare each aggregate's URLs once in a route definition within its shared area when multiple slices use them.
- Use the same route definitions for navigation links and programmatic navigation; avoid duplicating URL strings.
- Use a route template and builder for parameterized routes when that improves consistency.
- Keep a navigation service only when it implements real workflow behavior, such as preserving state, handling return URLs, or choosing a destination conditionally.

## 5) Localization

- Use resource files for application-owned text: labels, buttons, navigation, validation, errors, titles, and empty states.
- Localize page titles, headings, descriptions, loading and error states, and action text.
- Keep user-entered and domain content as application data; localize it only where the domain and product requirements call for translated content.
- Use stable codes or IDs for roles, permissions, statuses, and other authorization data. Never use translated display text for authorization decisions.
- Resolve language and fallback behavior at an appropriate data or API boundary when domain data is translated.
- Pages should receive ready-to-display localized data. Components should not independently load translations.
- Make translation completeness and missing-translation behavior explicit in any UI that manages translated data.

## 6) Responsive UI

- Design every slice for desktop and phone layouts from the start.
- Use one workflow with responsive presentation rather than separate mobile and desktop pages.
- Prefer cards or compact action menus for narrow-screen list views when tables become unusable.
- Use stacked forms, touch-friendly actions, and full-screen dialogs on phones.
- Avoid hover-only actions.
- Keep reusable responsive patterns in shared components and feature state in the slice.
