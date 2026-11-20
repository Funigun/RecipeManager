---
name: ui-planning
description: Plan the Blazor UI part of a User Story and write `.work/<id>/plan-ui.md`. Use when the user asks to plan a UI feature or slice. Identifies which backend endpoints are consumed. Read-only; never writes application code.
---

# UI planning

Turn a Story (and an approved contract, when one exists) into an ordered, implementable UI plan that names the backend it depends on.

## Before writing

1. Locate the target Story. Read every Story in `depends_on`, and read other Stories in the same Feature when the target lacks information required for this slice.
2. Read `.github/instructions/blazor.instructions.md` and follow its architecture.
3. Resolve the backend API to consume, in this order:
   1. `.work/<id>/contract.md` (must be `status: approved`).
   2. The OpenAPI document, when the project profile or solution provides one.
   3. Endpoint files under `RecipeManager.API/Features/<Domain>`, DTOs in API Shared, and validators in Shared Contracts.
4. If a required endpoint, field or link does not exist, record a `Backend gap` and stop planning the dependent tasks. Never invent an endpoint. Suggest running `backend-planning` or `contract-design` for it.
5. Reuse existing components, services and pages; inspect the UI projects before planning new ones.

## Plan format

Write the plan with this structure:

```markdown
---
status: draft
story: <repo-relative path of target Story>
depends_on: [contract.md]   # omit when there is no contract
---
# UI plan: <Story title>

## Sources
| Item | Origin | Why needed now |
|---|---|---|

## Consumed API
| Endpoint (method + route) | Origin (contract / OpenAPI / code path) | DTOs | Used by |
|---|---|---|---|

## Backend gaps
- <missing endpoint, field or link; blocks dependent tasks>

## Scope
<pages, components, navigation, render modes affected>

## Tasks
| ID | Description | Files/area | Depends on | Covers (AC-/R-) |
|---|---|---|---|---|
| U-1 | ... | ... | - | ... |

## Test plan
| Level | What is tested | Covers | Task |
|---|---|---|---|

## Risks and open questions
- ...
```

## Provenance (required)

- Every detail taken from a Story other than the target goes into `Sources`: Story path, the exact rule or criterion, and why it is needed at this stage.
- Conflicting or missing details go to `Risks and open questions`; never decide them silently.

## Rules for tasks

- Each task is small, verifiable and ordered; dependencies are explicit and acyclic.
- Every acceptance criterion of the target Story is covered by at least one task and test row. Report any that are not.
- Use shared validators from Shared Contracts; do not duplicate validation rules in the UI.
- Cover loading, empty, error and validation states for each screen.

## Finishing

1. Show consumed API, backend gaps, tasks and open questions.
2. Ask the user to approve or request changes. On approval, set `status: approved`. Never approve while backend gaps or open questions remain.

## Do not

- Write or modify application code.
- Plan backend work; report gaps instead.
- Add requirements that are not in a Story or the contract.
