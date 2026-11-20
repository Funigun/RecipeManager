---
name: contract-design
description: Design the API contract between backend and UI for a User Story slice and write `.work/<id>/contract.md`. Use when planning a fullstack feature, or when the user says "design the contract" or "define the API". Plans only; never writes application code.
---

# Contract design

Settle the backend/UI contract before either side is planned. The output is the single source of truth that `backend-planning` and `ui-planning` both reference.

## Before writing

1. Read the target Story and every Story in its `depends_on`. Read related Stories in the same Feature when the Story is silent about a detail the contract needs.
2. Read `wiki/domain` for the affected aggregates and existing endpoints under `RecipeManager.API/Features/<Domain>` and DTOs in API Shared. Reuse existing endpoints and DTOs when they fit.
3. Read existing validators in the Shared Contracts project; request validation lives there and is shared by backend and UI.
4. Record every fact taken from a Story other than the target one as a cross-story source (see Provenance).

## Contract content

Write `.work/<id>/contract.md` with front matter and these sections:

```markdown
---
status: draft
story: <repo-relative path of target Story>
---
# Contract: <Story title>

## Sources
| Item | Origin | Why needed now |
|---|---|---|

## Endpoints
| Name | Method + route | Auth policy | Request | Response | Status codes | New/Existing |
|---|---|---|---|---|---|---|

## DTOs
<field, type, nullability, validation rule per DTO>

## Validators (Shared Contracts)
<validators to add or change, with rules>

## HATEOAS links
<links returned per response and the conditions for them>

## Error cases
<NotFound, Authorization, Validation outcomes per endpoint and their ProblemDetails shape>

## Open questions
- <anything unresolved; blocks approval>
```

## Provenance (required)

- Anything decided or inferred from a Story other than the target must appear in `Sources` with the Story path, the exact rule or criterion used, and why this slice needs it at this stage.
- If sources conflict, do not choose silently: list the conflict under `Open questions`.
- If a needed detail exists in no Story, add it to `Open questions`; do not invent it.

## Finishing

1. Show the endpoint and DTO tables plus open questions.
2. Ask the user to approve or request changes. On approval, set `status: approved`. Never approve with open questions.

## Do not

- Write or modify application code.
- Plan implementation tasks; that belongs to `backend-planning` and `ui-planning`.
- Redefine an existing endpoint's behaviour without flagging it as a breaking change.
