---
name: grill-me
description: Interview the user one question at a time to turn a vague idea, feature, domain rule set or bug into resolved decisions. Use when the user says "grill me", has an unclear idea, or needs requirements or domain rules clarified before a PRD. Asks questions only; never writes the PRD or code.
disable-model-invocation: true
---

# Grill me

Resolve ambiguity through structured questions. Output is `decisions.md`, which `create-prd` consumes.

## Before asking

1. Read relevant code, `.github\copilot-instructions.md`. Never ask something the repo already answers. If the repo contradicts the user, say so and ask which is right.
2. Locate or create `.work/<id>/decisions.md` (ask the user for `<id>` or a short topic slug if unknown). If it exists, resume and skip decided questions; preserve its category grouping, decision IDs, and table of contents.
3. Restate the topic in one or two sentences and confirm it only if it is genuinely unclear.

## Question format (chat, always this shape)

```markdown
### Question <N> – <Title>

<Brief context: what needs deciding and why it matters.>

**Options**
- **A) <label>** – <what it enables; main trade-off or downstream consequence>
- **B) <label>** – <what it enables; main trade-off or downstream consequence>

**Recommended: <letter>** – <one-sentence reason tied to the goals or existing code.>

**This decision affects:** <known, material downstream product areas, when relevant.>

Reply with a letter, "accept" (take the recommendation), or free text.
```

Rules:
- Keep the first pass concise: include only context and consequences needed to make this decision. Use 2-4 options; free text is always allowed.
- For each option, state what it enables and its most important trade-off or downstream consequence. Do not invent a downside when there is no meaningful one, and do not turn this into an exhaustive pros/cons list.
- Include **This decision affects** only when there are known, material downstream product areas. Consider existing behavior or data transitions, ownership and permissions, edge cases, and user flows when relevant; do not imply an impact that the decisions or repository do not support.
- Show the recommendation immediately, with a brief reason. Do not write exhaustive pros/cons lists.
- Ask one question at a time. A batch of at most 3 is allowed only when the questions are independent.
- Do not use the `ask_user` form; stay in chat.
- After each answer, check whether the decision is clear and its important implications are understood. Briefly reflect a material consequence when it helps confirm the choice. A short answer is sufficient when it resolves the question; do not ask for extra detail just to lengthen the response.
- If a material ambiguity, consequence, or edge case remains, continue the same decision thread with one focused follow-up. Refer to the answer and ask only about what remains unclear; do not repeat the full question or option list. Ask one follow-up at a time, repeating only as needed to resolve a material issue.
- If the user says "enough" or defers an unresolved point, record it under Open questions or Assumptions and continue without treating it as decided.
- Let the user revise an earlier answer; update that decision in place and keep its ID instead of creating a duplicate.
- Order by dependency: scope and goals -> entities and relationships -> rules and invariants -> edge cases and failure modes -> permissions (User/Admin) -> non-goals -> UI and non-functional needs.
- Keep the interview focused on product decisions needed to define user outcomes. Do not exhaustively elicit screens, interactions, or visual details; defer lower-level UI exploration to the optional prototype step unless a detail changes the user outcome or a material product rule.
- Politely point out a material conflict or trade-off and its consequence; ask only for the clarification needed to resolve it.

## Persisting

After each answer, update `.work/<id>/decisions.md` before asking the next question. Record a resolved answer as a decision; keep unresolved details in Open questions or Assumptions until clarified:

```markdown
---
status: draft            # draft | approved
topic: <topic>
---
## Table of contents

- [<Topic>](#<topic-anchor>)

## Decisions

### <Topic>

- <u>D-01 — <Decision title> (User)</u>

  **Decision:** <resolved choice>

  **Reason:** <why this choice was made>

## Open questions

## Assumptions

## Out of scope

## Glossary
```

Keep category headings unindented, then list each decision one level below its category. Group decisions by model, category, or capability rather than by interview order; keep headings concise and update the table of contents when headings change. Indent each Decision and Reason paragraph as continuation text within its decision list item. Use one entry per distinct decision, and keep each entry's original ID stable when updating or regrouping the document. Do not use a decision table. Assign new decisions the next unused numeric ID; never renumber existing decisions. Preserve the source and rationale for each decision.

Record "accept" answers as `(Recommended)` in the heading, and free-text answers as `(User)` with the user's reasoning.

## Finishing

Stop when no material open questions remain, or the user says "enough". Record leftover unknowns under Assumptions or Open questions. Then:

1. Show a short summary of decisions, assumptions, out-of-scope items and glossary.
2. Ask for approval. On approval set `status: approved`; changes go back into the question loop.
3. Suggest the next step: `create-prd`, which must not run on a non-approved file.

## Do not

- Write the PRD, plans, or code.
- Ask questions answerable from the repo.
- Batch many questions or hide the recommendation.
