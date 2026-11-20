---
name: backend-planning
description: Plan the backend part of a User Story in RecipeManager.API and write `.work/<id>/plan-backend.md`. Use when the user asks to plan a backend feature or slice. Read-only; never writes application code.
---

# Backend planning

Turn a Story (and an approved contract, when one exists) into an ordered, implementable backend plan.

## Before writing

1. Locate the target Story. Read every Story in `depends_on`, and read other Stories in the same Feature when the target lacks information required for this slice.
2. If `.work/<id>/contract.md` exists, it must be `status: approved`; otherwise stop and suggest `contract-design`. Do not redefine anything the contract settles.
3. If there is no contract (backend-only), define the endpoint shape inside the plan under `Endpoints`.
4. Read `wiki/domain` and `wiki/adr` for affected aggregates and decisions, `.github/copilot-instructions.md`, and a reference feature in `RecipeManager.API/Features` (see `backend-feature-slice`).
5. Inspect existing code to decide what is new vs reused. Do not plan what already exists.

## Plan format

Write the plan with this structure:

```markdown
---
status: draft
story: <repo-relative path of target Story>
depends_on: [contract.md]   # omit when there is no contract
---
# Backend plan: <Story title>

## Sources
| Item | Origin | Why needed now |
|---|---|---|

## Scope
<what is in and out of this slice>

## Domain and data changes
<entities, EF configuration, migrations, seed data>

## Endpoints
<only when there is no contract; otherwise link to it>

## Tasks
| ID | Description | Files/area | Depends on | Covers (AC-/R-) |
|---|---|---|---|---|
| B-1 | ... | ... | - | ... |

## Test plan
| Level (unit / integration / architecture) | What is tested | Covers | Task |
|---|---|---|---|

## Risks and open questions
- ...
```

## Provenance (required)

- Every detail taken from a Story other than the target goes into `Sources`: Story path, the exact rule or criterion, and why it is needed at this stage.
- Details that conflict across Stories or exist in none go to `Risks and open questions`; never decide them silently.
- Tasks that exist only because of another Story must cite it in `Description`.

## Rules for tasks

- Each task is small, verifiable and ordered; dependencies are explicit and acyclic.
- Every acceptance criterion of the target Story is covered by at least one task and at least one test row. Report any that are not.
- Respect handler ordering and layout from `backend-feature-slice`: authorization, then not-found check, then validation, then operation.
- Name the narrowest build and test command for each group of tasks.

## Finishing

1. Show tasks, coverage and open questions.
2. Ask the user to approve or request changes. On approval, set `status: approved`. Never approve with open questions.

## Do not

- Write or modify application code.
- Add requirements that are not in a Story or the contract.
- Plan UI work.
