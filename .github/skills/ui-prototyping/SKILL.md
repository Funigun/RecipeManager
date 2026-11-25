---
name: ui-prototyping
description: Create an optional HTML prototype from a feature brief or PRD and the host project's design guidance, then propose UI/UX requirement updates for approval.
---

# UI prototyping

Create a visual prototype to help clarify feature UI requirements before generating User Stories. The prototype is an exploration artifact; the PRD remains the source of approved requirements.

## Rules

- Use decoration only when it supports meaning. 
- Prefer clear hierarchy, restrained color, consistent spacing, and the simplest control that fits the task. 
- Reuse the product’s existing components and visual language.
- Don’t add dashboard filler or decorative effects.

## Clarify intent

- Treat questions, reactions, and brainstorming about an existing prototype (for example, “I’m not sure I like this” or “what could work?”) as design discussion, not permission to edit files.
- For discussion requests, analyze the relevant brief and prototype, explain what is and is not working, and offer a small set of possible directions. Do not edit until the user asks to see or apply a direction.
- Before a substantive redesign, align on its direction and scope. After the user approves a direction, make only the agreed changes; preserve decisions already made and do not broaden the redesign.
- Phrases such as “let’s see what we get” authorize a prototype only for the most recently agreed direction, not an unrestricted redesign.

## Before writing

1. Identify the feature and its source brief or PRD. If the user points to a file, use it. If multiple candidate PRDs exist and the target is unclear, ask which one.
2. Read the selected brief or PRD, its related decisions, and relevant existing feature/UI references. Use the PRD and decisions to understand intended behavior; do not infer product requirements from visual references alone.
3. Locate the host project's design guidance. Prefer an explicit user-provided reference; otherwise inspect project documentation and existing mockups for the canonical design source. Do not assume a RecipeManager-specific path or copy its design system into another project.
4. Choose a draft output location. Use the host project's established scratch/work area. In RecipeManager, place prototype files under `.work/<id>/ui-prototype/`, alongside the related work item. If no suitable location or feature identifier can be established, ask before creating a new directory structure.
5. Review any existing prototype artifacts in the chosen location and update only those that belong to this feature. Do not overwrite unrelated work.

## Define the mock

Start with the page route (when applicable), intended audience and role, and the page's goal. Then choose the page pattern; combine patterns only when the page needs them.

- **CRUD or detail page:** Identify the model and relevant fields, form shape, primary actions and their outcomes, and any necessary dialogs.
- **List or collection page:** Identify the model, table/grid/list shape and information shown, relevant search/filter/sort/grouping behavior, row or item actions, and any necessary dialogs.
- **Landing or concept page:** Identify the audience, core promise and benefit priorities, call to action, and section order. Agree on the content outline before choosing detailed visualizations.
- **Workflow page:** Define the steps, decisions, states, transitions, and visible outcomes.

For dialogs, specify what opens them, what the user can do, and what happens on completion or cancellation. Include loading, empty, error, validation, and role-specific states only when relevant and established by the source material or clearly labeled as proposals.

## Create the prototype

- Produce HTML files only; include CSS and small amounts of JavaScript in the HTML when needed to demonstrate an interaction.
- Keep the artifact easy to open and review. Prefer self-contained markup and styling; use existing project assets or libraries only when they materially help represent the design reference.
- Match the host project's design guidance and reuse relevant existing patterns. Keep shared design tokens in their source documents; do not redefine the global design system in the prototype.
- Cover only the screens and flows relevant to the selected feature. Use the PRD's user outcomes and constraints to guide content, controls, navigation, and responsive behavior.
- Keep the prototype within the selected PRD context and relevant flow; do not attempt to inventory or specify the whole product UI in one prototype.
- Include loading, empty, error, validation, and role-specific states only when the source requirements establish them or the prototype explicitly labels them as proposals.
- Do not add backend calls, production components, application code, or build configuration. This skill creates reviewable HTML prototypes, not production UI.
- When a visual choice or interaction is not established by the source material, make it visibly provisional and list it as a proposal or open question. Never silently turn a mockup detail into a product requirement.
- This skill is optional. If the user redirects to direct PRD updates or product UI implementation based on an existing feature, stop the prototype workflow and handle that as a separate task under the relevant project instructions. Do not require a prototype step or introduce a special “build as” mode.

## Use prototype findings in the PRD

1. Compare the prototype with the source brief/PRD and decisions. Separate already-approved behavior from new proposals and unresolved questions.
2. Present a concise summary with:
   - HTML prototype file paths and the screens/flows they demonstrate.
   - UI decisions reflected from the source requirements.
   - Proposed UI/UX requirements and any new candidate acceptance criteria.
   - Assumptions, conflicts, and open questions.
3. Show the exact proposed PRD changes before editing. Use the UI and UX needs section for screen and flow context. Put approved user actions and visible outcomes in the acceptance criteria of the related existing PRD story card; add a PRD story card only for a distinct, independently demonstrable user outcome.
   - If the input is a feature brief and no PRD exists, provide proposed UI/UX requirement text for later use; do not create a PRD.
4. Do not add unsupported behavior, constraints, validation, accessibility requirements, or non-functional requirements. A mockup detail is not a requirement unless explicitly approved. Preserve existing rule and acceptance-criterion IDs; allocate new IDs only after the user approves adding those requirements, following the PRD's existing conventions.
5. Apply the proposed PRD changes only after the user explicitly approves them. If the user declines, keep the prototype as a draft and do not modify the PRD.
6. Do not generate User Stories or Tasks. The approved PRD is the input to `prd-to-items`.

## Finishing

Show the prototype paths, a short summary of the screens/flows, the proposed or applied PRD changes, and unresolved questions. Ask for approval before applying PRD edits if approval has not already been given.

## Do not

- Require this skill for every feature or UI change.
- Treat mockup details as approved requirements without user approval.
- Modify shared design guidance or promote a draft prototype to a shared mockup library without an explicit request.
- Implement production UI or modify application code.
- Create or update backlog User Stories.
