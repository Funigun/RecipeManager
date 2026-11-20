---
name: domain-designer
description: Prepare or update the domain model shape in `wiki/domain` from an approved `.work/<id>/prd.md`, or from a direct user description. Use when the user says "design the domain", "update domain model" or after create-prd is approved. Writes domain documentation only; never writes application code or plans.
---

# Domain designer

Describe the domain shape (entities, value objects, relationships, invariants, lifecycle) in `wiki/domain`. Runs between `create-prd` and `prd-to-items`, and can also be used on its own.

## Before writing

1. Determine input:
   - With `<id>`: read `.work/<id>/prd.md`. If missing or `status` is not `approved`, stop and suggest `create-prd`.
   - Standalone: use the user's description; ask only for what is needed to avoid guessing.
2. Read existing `wiki/domain` files, `.github\copilot-instructions.md` and the relevant entities in `RecipeManager.API/Features` so terminology and structure match. Create `wiki/domain` if missing.
3. If a domain file for the feature exists, update it in place; do not duplicate or rewrite unrelated parts.

## Rules

- Source of truth is the PRD (`R-` rules, glossary, "Documentation targets") or the user's statement. Invent nothing; unresolved gaps go to **Open questions** (material ones: send the user to `grill-me`).
- Keep it simple: one Markdown file per feature/domain area (e.g. `wiki/domain/<feature>.md`), matching existing file naming.
- Model the business shape only: no DB schema, endpoints, DTOs, class code or task breakdowns.
- Every PRD `R-` rule that concerns the domain must be traceable to an invariant or relationship; cite the ID.
- Show changes to existing entities explicitly (added/changed/removed) so the diff is reviewable.
- Use existing domain terms; flag renames.

## Output: `wiki/domain/<feature>.md`

```markdown
# <Feature> domain

## Purpose
<1-2 sentences.>

## Entities and value objects
| Name | Kind (entity/value object) | Description | Key attributes |
|------|----------------------------|-------------|----------------|

## Relationships
<Short list or a mermaid `erDiagram`/`classDiagram`.>

## Invariants and rules
| Rule | Applies to | PRD ref |
|------|------------|---------|

## Lifecycle / states
<Only if relevant.>

## Open questions
```

## Finishing

1. Summarize what was added/changed in the domain (entities, relationships, invariants) and open questions.
2. Ask for approval; apply requested changes and re-show.
3. If run from a PRD, suggest the next step: `prd-to-items`. If the PRD's "Documentation targets" mentions ADRs, remind the user they are not handled by this skill.

## Do not

- Run from a non-approved PRD.
- Write application code, plans, tasks or work items.
- Silently resolve gaps or contradictions.
