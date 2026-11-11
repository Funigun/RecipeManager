---
name: create-prd
description: Turn an approved `.work/<id>/decisions.md` (from grill-me) into `.work/<id>/prd.md`. Use when the user says "create PRD" or after grill-me is approved. Writes requirements only; never plans or writes code. Refuses to run on a non-approved decisions file.
---

# Create PRD

Convert resolved decisions into a testable product requirements document. The PRD says **what and why**, never **how**. Planning skills consume it later; `update-documentation` uses it to update `wiki/domain` and `wiki/adr`.

## Before writing

1. Locate `.work/<id>/decisions.md` (ask for `<id>` if unknown). If missing or `status` is not `approved`, stop and suggest `grill-me`.
2. Read `wiki/domain`, `wiki/adr`, `.github\copilot-instructions.md`, and relevant code to align terminology and to fill the "Impact on existing system" section.
3. If `.work/<id>/prd.md` exists, update it instead of overwriting; keep IDs stable.

## Rules

- Invent nothing. Anything not covered by decisions goes to **Open questions**; if material, send the user back to `grill-me`.
- Every decision must appear in at least one section. Every rule and criterion gets a stable ID (`R-01`, `AC-01`) that is never reused or renumbered.
- Acceptance criteria must be testable (Given/When/Then or a clear observable outcome).
- No implementation detail: no class names, endpoints, table schemas or task breakdowns.
- Use the glossary from `decisions.md` and the existing domain terms.

## Output: `.work/<id>/prd.md`

```markdown
---
status: draft            # draft | approved
topic: <topic>
source: decisions.md
---
# <Title>

## 1. Summary
<2-3 sentences: problem and desired outcome.>

## 2. Goals and non-goals
- Goals:
- Non-goals:

## 3. Users and permissions
| Role | Can | Cannot |
|------|-----|--------|

## 4. Domain rules
| ID | Rule | Entities | Source decision # |
|----|------|----------|-------------------|

## 5. User stories and acceptance criteria
### US-01 <title>
As a <role> I want <capability> so that <value>.
- AC-01: <testable criterion>

## 6. UI and UX needs
<Screens, flows, states (empty, error, loading), role differences. Blazor WASM level of detail only.>

## 7. Non-functional requirements
<Security, performance, availability, data retention, hosting or Azure constraints.>

## 8. Impact on existing system
<Affected features, data migration, breaking changes, refactoring needed.>

## 9. Open questions and assumptions

## 10. Documentation targets
<wiki/domain and wiki/adr files to create or update, and what changes.>
```

## Finishing

1. Show a short summary: counts of rules, stories and criteria, plus open questions.
2. Ask for approval. On approval set `status: approved`; requested changes are applied and re-shown.
3. Suggest the next step: `update-documentation` (domain docs and ADRs), then planning.

## Do not

- Run on non-approved decisions.
- Write plans, tasks or code.
- Silently resolve gaps or contradictions.
