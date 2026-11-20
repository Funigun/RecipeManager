---
name: prd-to-items
description: Generate draft User Story files from an approved `.work/<id>/prd.md` in the repository work-item hierarchy. Use when the user says "create work items", "split PRD" or "prepare backlog". Generates Stories only; never generates Tasks or publishes to an external provider.
---

# PRD to items

Turn an approved PRD into User Story documents in the repository work-item hierarchy. These files are the source of truth; the work-item CLI reads them but does not create or edit them.

## Before writing

1. Locate `.work/<id>/prd.md`. If missing or `status` is not `approved`, stop and suggest `create-prd`.
2. Read `wiki/domain` (if the PRD introduces domain changes not yet reflected there, suggest running `domain-designer` first) and inspect `Requirements/Epics/` for the relevant Epic and Feature directories and existing Story files. Use existing directory names and conventions where applicable.
3. Determine the target Epic and Feature for each Story. If either cannot be determined from the PRD and existing hierarchy, ask the user. Create missing Epic, Feature, or `User stories` directories only after the target names are clear. Do not create metadata documents for Epic or Feature directories.

## Hierarchy and coverage

- The hierarchy is `Requirements/Epics/<Epic>/Features/<Feature>/User stories/<Story-slug>.md`.
- Epic and Feature directories are grouping-only. Do not create Epic or Feature work-item documents, descriptions, or statuses.
- Generate one Markdown file per User Story. A Story is a vertical slice that can be implemented and demonstrated on its own.
- Generate no Task items.
- A work item's ID is its repository-relative path. Do not add a separate key or numeric ID. Use repository-relative Story file paths in `depends_on`.
- Cover every `R-` rule and `AC-` criterion in the PRD with at least one Story. Report uncovered IDs; do not invent Stories or requirements to hide gaps.
- Set dependencies only when required by the PRD or its domain rules. Keep dependencies acyclic; do not add arbitrary related links.

## Story file format

Use this format for new Story files:

```markdown
---
status: draft
depends_on: []
---
# <Story title>

As a <role>, I want <capability> so that <value>.

## Acceptance criteria
- <testable criterion>
```

- `status` must be one of `draft`, `ready`, `in_progress`, or `done`. New Stories start as `draft`.
- `depends_on` is a list of repository-relative paths to other Story files; use `[]` when there are no dependencies.
- Do not add an assignee field, separate work-item ID, provider metadata, or generated Task content.
- Preserve the file path and current status of an existing Story when updating it. Do not replace an existing file for an unrelated Story; ask the user if its identity or intended update is unclear.

## Finishing

1. Show a table of the Epic/Feature grouping, generated Stories, covered `R-`/`AC-` IDs, and any uncovered IDs.
2. Ask the user to approve or request changes to the Story set. This is one approval for the set, not a separate Feature approval.
3. On approval, set each generated Story's `status` to `ready`. Apply requested changes and show the updated summary. Do not change existing Stories' statuses as part of approving the generated set.
4. The work-item CLI may be used to list, search, or inspect the files. It is read-only and must not be used to create, edit, or delete work-item files.

## Do not

- Run on a non-approved PRD.
- Generate Task items or Epic/Feature metadata documents.
- Add an assignee field, provider integration, or arbitrary related links.
- Publish work items or call `gh`, `az`, or any external provider API.
- Add requirements that are not in the PRD.
