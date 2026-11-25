---
name: prd-to-items
description: Generate draft User Story files from an approved context or Feature PRD under `Documentation/ProductBacklog`. Use when the user says "create work items", "split PRD" or "prepare backlog". Generates Stories only; never generates Tasks or publishes to an external provider.
---

# PRD to items

Turn an approved PRD into User Story documents in the repository work-item hierarchy. These files are the source of truth; the work-item CLI reads them but does not create or edit them.

## Before writing

1. Locate the approved PRD. Single-context PRDs may live at `Documentation/ProductBacklog/<Context>/prd.md`; domain-context PRDs live at `Documentation/ProductBacklog/<Domain>/<Context>/prd.md` (for example, `recipe-domain/ingredients/prd.md` or `users-domain/identity-management/prd.md`); existing Feature PRDs may still use `Documentation/ProductBacklog/Epics/<Epic>/Features/<Feature>/prd.md`. If the user identifies a context or Feature, use that PRD; otherwise ask which one when multiple PRDs are available. If the PRD is missing or `status` is not `approved`, stop and suggest preparing or approving it with `create-prd`.
2. For a domain-context PRD, read its domain root's `overview.md` and `decisions.md` when present, its adjacent `decisions.md`, any context PRDs that own referenced cross-context rules, `wiki/domain`, the PRD's UI and UX needs when present, and the selected Story templates. For a single-context or existing Feature PRD, read its adjacent `decisions.md`, `wiki/domain`, the PRD's UI and UX needs when present, and the selected Story templates. Follow cross-context `R-` references to their owning PRDs. If the PRD introduces domain changes not yet reflected in `wiki/domain`, suggest running `domain-designer` first.
3. Inspect the selected PRD folder's existing `User stories/` files before writing. Create only the `User stories/` directory when needed; do not create or rename ProductBacklog context, Epic, or Feature folders or add metadata documents for them.

## Hierarchy and coverage

- Single-context Stories belong under `Documentation/ProductBacklog/<Context>/User stories/<Story-slug>.md`; domain-context Stories belong under `Documentation/ProductBacklog/<Domain>/<Context>/User stories/<Story-slug>.md`. Existing Feature Stories remain under `Documentation/ProductBacklog/Epics/<Epic>/Features/<Feature>/User stories/<Story-slug>.md`.
- `.work/` is the local workspace for unpromoted ideas and drafts, not the source or destination for generated backlog Stories. Promote an approved PRD into its ProductBacklog context or Feature before running this skill.
- Domain, context, Epic, and Feature directories are grouping-only. Do not create domain, context, Epic, or Feature work-item documents, descriptions, or statuses.
- Generate one Markdown file per User Story. A Story is a vertical slice that can be implemented and demonstrated on its own.
- Keep Stories organized around user outcomes, not frontend/backend layers. Include approved UI behavior in the relevant Story when it supports that outcome; create a UI-focused Story only when the PRD defines an independently demonstrable user outcome with acceptance criteria.
- Use the UI and UX needs section only as screen and flow context for the approved Story criteria. Do not create Stories or behavior from screens, prototypes, or visual details alone.
- If a PRD story combines independently demonstrable outcomes, split it only when the PRD provides enough detail to define each outcome; preserve the source criteria IDs and do not invent missing behavior.
- Generate no Task items.
- A work item's ID is its repository-relative path. Do not add a separate key or numeric ID. Use repository-relative Story file paths in `depends_on`.
- Map every relevant `R-` rule and `AC-` criterion to a Story or explicitly classify it as a cross-cutting, deferred, out-of-scope, or transition constraint. Do not create Stories for constraints that have no user outcome. Report any unclassified IDs; do not invent Stories or requirements to hide gaps.
- Set dependencies only when required by the PRD or its domain rules. Keep dependencies acyclic; do not add arbitrary related links.

## Choose a Story template

- Read the PRD's user-story section, its referenced domain rules, and any explicit open questions. Support both concise story cards with related-rule references and older PRDs that place acceptance criteria directly under each story.
- Choose the template that best describes the primary behavior. Use the most specific matching template:
  1. `calculation-aggregation.md` for formulas, scaling, normalization, aggregation, or quantity fallbacks.
  2. `stateful-workflow.md` for multi-step flows with pending states, verification, recovery, or other state transitions.
  3. `collection-behavior.md` for collection membership, cardinality, uniqueness, date/category assignment, or multiple entries.
  4. `entity-lifecycle.md` for creating, updating, mapping, archiving, deleting, ownership, or required-field behavior.
  5. `capability.md` for simple browse, select, or single-action behavior not covered by a more specific template.
- Templates live in `.github/templates/user story/`. Read and apply one template per generated Story. If a Story includes secondary behavior, include it in the chosen template only when it is part of the same vertical slice. If the outcomes are independently demonstrable, split them when the PRD supports that split.
- Use acceptance criteria and domain rules to make the Story self-contained and testable. Carry through the source Story's approved UI actions and visible outcomes when relevant. Include validation, edge cases, and failure outcomes only when the PRD establishes them. Preserve unresolved product behavior as an open question; never infer security, validation, timing, or error-handling requirements.
- Remove unused optional sections and all template placeholders from generated files. Do not duplicate the whole PRD rule text when a concise criterion and rule reference are sufficient. Keep source `AC-` and `R-` IDs in a traceability section.

```markdown
---
status: draft
depends_on: []
---
# <Story title>
```

- `status` must be one of `draft`, `ready`, `in_progress`, or `done`. New Stories start as `draft`.
- `depends_on` is a list of repository-relative paths to other Story files; use `[]` when there are no dependencies.
- Do not add an assignee field, separate work-item ID, provider metadata, or generated Task content.
- Preserve the file path and current status of an existing Story when updating it. Do not replace an existing file for an unrelated Story; ask the user if its identity or intended update is unclear.

## Finishing

1. Show a table of the selected context or Epic/Feature grouping, generated Stories and selected templates, covered `R-`/`AC-` IDs, non-story constraints, and any unclassified IDs.
2. Ask the user to approve or request changes to the Story set. This is one approval for the set, not a separate context or Feature approval.
3. On approval, set each generated Story's `status` to `ready`. Apply requested changes and show the updated summary. Do not change existing Stories' statuses as part of approving the generated set.
4. The work-item CLI may be used to list, search, or inspect the files. It is read-only and must not be used to create, edit, or delete work-item files.

## Do not

- Run on a non-approved PRD.
- Generate Task items or Epic/Feature metadata documents.
- Add an assignee field, provider integration, or arbitrary related links.
- Publish work items or call `gh`, `az`, or any external provider API.
- Add requirements that are not in the PRD.
