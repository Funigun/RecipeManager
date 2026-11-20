---
name: grill-me
description: Interview the user one question at a time to turn a vague idea, feature, domain rule set or bug into resolved decisions. Use when the user says "grill me", has an unclear idea, or needs requirements or domain rules clarified before a PRD. Asks questions only; never writes the PRD or code.
---

# Grill me

Resolve ambiguity through structured questions. Output is `decisions.md`, which `create-prd` consumes.

## Before asking

1. Read relevant code, `.github\copilot-instructions.md`. Never ask something the repo already answers. If the repo contradicts the user, say so and ask which is right.
2. Locate or create `.work/<id>/decisions.md` (ask the user for `<id>` or a short topic slug if unknown). If it exists, resume and skip decided questions.
3. Restate the topic in one or two sentences and confirm it only if it is genuinely unclear.

## Question format (chat, always this shape)

```markdown
### Question <N> – <Title>

<Context: 1-3 paragraphs. Why it matters, what depends on it (domain, API, UI, data migration).>

**Options**
- **A) <label>** – <consequence>
- **B) <label>** – <consequence>
- **C) <label>** – <consequence>

**Recommended: <letter>** – <reason tied to the stated goals and existing code.>

Reply with a letter, "accept" (take the recommendation), or free text.
```

Rules:
- Show the recommendation immediately in every question, with its reason.
- Options must carry consequences, not just labels. Use 2-4 options; free text is always allowed.
- Ask one question at a time. A batch of at most 3 is allowed only when the questions are independent.
- Do not use the `ask_user` form; stay in chat.
- If the reply is ambiguous or matches no option, re-ask briefly instead of guessing.
- Order by dependency: scope and goals -> entities and relationships -> rules and invariants -> edge cases and failure modes -> permissions (User/Admin) -> non-goals -> UI and non-functional needs.
- Challenge weak or conflicting answers once, politely, with the consequence.

## Persisting

After each answer, update `.work/<id>/decisions.md` before asking the next question:

```markdown
---
status: draft            # draft | approved
topic: <topic>
---
## Decisions
| # | Title | Decision | Rationale | Source (user / recommended) |
|---|-------|----------|-----------|-----------------------------|

## Open questions

## Assumptions

## Out of scope

## Glossary
```

Record "accept" answers as `recommended`, and free-text answers as `user` with the user's reasoning.

## Finishing

Stop when no material open questions remain, or the user says "enough". Record leftover unknowns under Assumptions or Open questions. Then:

1. Show a short summary of decisions, assumptions, out-of-scope items and glossary.
2. Ask for approval. On approval set `status: approved`; changes go back into the question loop.
3. Suggest the next step: `create-prd`, which must not run on a non-approved file.

## Do not

- Write the PRD, plans, or code.
- Ask questions answerable from the repo.
- Batch many questions or hide the recommendation.
