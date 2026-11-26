---
name: ui-feature-slice
description: Implement a Blazor UI vertical feature slice in RecipeManager.UI using the repository's slice, API, and presentation conventions.
---

# UI feature slice

Use this skill when implementing or restructuring a feature in `RecipeManager.UI`. Build a complete, focused UI slice from the user's request and its approved requirements; do not turn implementation into a planning exercise.

## Before editing

1. Read `.github/instructions/blazor.instructions.md` and follow its architecture.
2. Read the target User Story and its acceptance criteria when provided. If a UI plan accompanies it, use that plan to determine scope and file changes. Treat an unapproved plan as guidance, not approval of unresolved product decisions.
3. Resolve the backend API contract in this order:
   1. `.work/<id>/contract.md`, when approved.
   2. The relevant OpenAPI document, when available.
   3. Endpoint mappings, request/response types, and validators in the owning API project.
4. Inspect existing UI slices, API clients, shared components, localization resources, authorization patterns, and test infrastructure that are relevant to the change. Reuse established patterns; do not assume an infrastructure or project exists.
5. If a required endpoint, contract field, link, or product behavior is missing or ambiguous, do not invent it. Ask for clarification when a product decision blocks implementation; otherwise implement only the supported scope and report the gap.

## Implement the slice

- Organize UI by module, aggregate, and user action, following the Blazor instructions. Keep each workflow in its own slice root; do not combine Create, Read, and Edit orchestration behind a mode parameter.
- Keep pages focused on routes, layout, authorization, localized titles, and rendering their slice root. The slice root owns workflow state, API calls, validation, loading and error handling, and post-action navigation.
- Keep presentational components free of API calls, data loading, navigation, and operation selection. Pass data, validation/display state, and callbacks through parameters.
- Reuse shared forms only when their fields and validation are intentionally common. Keep materially different workflows, models, and validation separate.
- Use existing API clients and shared HTTP infrastructure. Map transport contracts to UI view models where presentation needs differ; map form models to request contracts at the workflow boundary. Do not invent endpoints or couple visual components to HTTP transport.
- Use shared validators from Shared Contracts instead of duplicating validation rules. Preserve API validation failures in the appropriate form state.
- Keep load failures separate from submit failures, preserve loaded content and form state on submit failure, and reset submitting state on success and failure. Show localized, user-safe errors; do not expose exception details or server diagnostics.
- Follow existing authentication and authorization conventions. UI authorization controls presentation, not server-side access enforcement.
- Use resource files for application-owned text and follow existing responsive patterns for phone and desktop layouts. Avoid introducing a hosting-model or broad architecture change.
- Make only changes needed for the requested slice. Do not implement backend work, change shared design guidance, or create planning/backlog artifacts unless explicitly requested.

## Verification

- Cover the relevant acceptance criteria, including applicable loading, empty, validation, and error states.
- Inspect the existing UI test infrastructure and add focused tests only where a suitable harness exists. Do not imply automated UI tests or add a new test project without an explicit need; use manual browser verification when no suitable harness exists.
- Run the narrowest relevant build and tests for the changed UI. Resolve failures caused by the change and report any checks that could not be run.

## Finishing

Summarize the implemented UI behavior and changed files, report backend gaps or unresolved decisions, and state the build/test or manual verification performed.
