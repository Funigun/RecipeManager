---
name: create-prd
description: Turn an approved `.work/<id>/decisions.md` (from grill-me) into `.work/<id>/prd.md`. Use when the user says "create PRD" or after grill-me is approved. Writes requirements only; never plans or writes code. Refuses to run on a non-approved decisions file.
---

# Create PRD

Convert resolved decisions into a testable product requirements document. The PRD says **what and why**, never **how**. `domain-designer` uses it to update `wiki/domain`; planning skills consume it later.

## Before writing

1. Locate `.work/<id>/decisions.md` (ask for `<id>` if unknown). If missing or `status` is not `approved`, stop and suggest `grill-me`.
2. Read `.github\copilot-instructions.md`, and relevant code to align terminology and to fill the "Impact on existing system" section.
3. If `.work/<id>/prd.md` exists, update it instead of overwriting; keep IDs stable.

## Rules

- Invent nothing. Anything not covered by decisions goes to **Open questions**; if material, send the user back to `grill-me`.
- Keep each PRD scoped to the feature or context represented by its decisions. Reference cross-context requirements and their owning PRDs instead of combining separate contexts into one PRD.
- Every decision must appear in at least one section. Group domain rules under concise model/category headings that follow the decisions document; do not put domain rules in a table.
- Give each domain rule a stable ID (`R-01`), a short title, its relevant entities, and source decision IDs; mark approved assumptions as assumptions. Keep each rule concise and use nested bullets when it contains distinct statements.
- Keep existing `R-` and `AC-` IDs stable when updating an approved PRD. Never reuse or renumber them; moving an acceptance criterion to a product constraint does not change its ID.
- Assign new `R-` and `AC-` IDs using the next unused number in each series; never recycle an ID that was removed or moved.
- Acceptance criteria must be testable (Given/When/Then or a clear observable outcome).
- Use concise, focused story cards with one user outcome per story. Split stories when actors or independently demonstrable outcomes differ; don't combine unrelated behavior just because it concerns the same model.
- Capture approved UI actions and user-visible outcomes in the acceptance criteria of the relevant story cards. Keep **UI and UX needs** concise and context-local: summarize known screens and flows, and include role differences or states only when established. Do not turn this section into an exhaustive screen inventory or infer behavior from design references. Lower-level interaction details may be explored with the optional prototype; leave unresolved product behavior open rather than blocking on visual detail.
- Keep the PRD story format lightweight and consistent. Reference related `R-` rules rather than repeating their full text. Do not use the detailed work-item templates in `.github/templates/user story/` in the PRD; `prd-to-items` chooses those later.
- Put deferred, scope, transition, and cross-cutting requirements that have no user outcome under **Product constraints (not user stories)** instead of forcing them into a User Story. Keep any existing `AC-` IDs and reference the related `R-` rules.
- Include validation and edge-case criteria only when the decisions specify them. Do not invent field constraints, error behavior, security policy, or workflow mechanics; record unresolved behavior under **Open questions and assumptions**.
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
| <role> | <ul><li><capability></li><li><capability></li></ul> | <ul><li><restriction></li></ul> |

## 4. Domain rules
### <Model or category from decisions.md>
- **R-01 — <short rule title>** *(Entities: <entities>; Source: D-01, D-02)*
  - <Concise rule statement>
  - <Additional distinct rule statement, if needed>

## 5. User stories
Each card has one actor and outcome. Keep acceptance criteria observable and link the domain rules they depend on.

### US-01 — <user outcome>
As a <role>, I want <capability> so that <value>.

**Acceptance criteria**
- **AC-01:** <testable behavior or outcome>

**Related domain rules:** R-01, R-02

### Product constraints (not user stories)
- <Deferred, scope, transition, or cross-cutting requirement without a distinct user outcome>

**Related domain rules:** R-03

## 6. UI and UX needs
<Concise screens and flows for this PRD context. Reference related stories and acceptance criteria for behavior; include only established role differences and states. Leave lower-level interaction details for optional prototyping.>

## 7. Non-functional requirements
<Security, performance, availability, data retention, hosting or Azure constraints.>

## 8. Impact on existing system
<Affected features, data migration, breaking changes, refactoring needed.>

## 9. Open questions and assumptions

## 10. Documentation targets
<wiki/domain and wiki/adr files to create or update, and what changes.>
```

## Finishing

1. Show a short summary: counts of rules, stories, criteria, product constraints, plus open questions.
2. Ask for approval. On approval set `status: approved`; requested changes are applied and re-shown.
3. Suggest the next step: `domain-designer` (domain shape in `wiki/domain`), then `prd-to-items`.

## Do not

- Run on non-approved decisions.
- Write plans, tasks or code.
- Silently resolve gaps or contradictions.
