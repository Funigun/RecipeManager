# AI Development Workflow – Analysis Summary

Status: analysis only, nothing implemented. Source proposal: `1) Copilot instructions.txt`.
Scope decision: keep everything in this repo for now (no global skills or plugin yet).

## 1. Key decisions so far

- Skills are the primary unit and must work standalone (no agent, no `.work/` required).
- Agents are thin orchestrators: they compose skills, manage handoffs and human gates.
- Skills and agents contain no project architecture knowledge. A `project-discovery` skill produces a project profile; skills read the profile and instruction files.
- Layer skills: `backend-development`, `ui-development`, `testing`, plus planning skills.
- Reviewer agent is read-only and ends in human triage. No automatic reviewer -> implementer loop.
- Gates and triage are hybrid: file is the source of truth, chat is a shortcut, agent syncs both.
- Gate approval is tracked via a `status` front-matter field in each `.work/<id>/*.md` file.
- Later option: move generic skills to personal skills (`~/.copilot/skills`) or a plugin for reuse/sharing. Not for now.

## 2. Project profile schema

Location: `docs/engineering/project-profile.md` (or under `wiki/`). Facts and pointers only; rules live in instruction files.

- `meta`: name, purpose, last-analyzed commit, profile version
- `layers[]`: id, path, stack, architecture pattern, entry points, instruction file, reference feature
- `commands`: restore, build, test (unit/integration/architecture) with scope filters, format/lint, run, prerequisites (e.g. Docker)
- `testing`: frameworks, projects per layer, test types and when each applies
- `quality`: analyzers, `.editorconfig`, global build files, where to add packages
- `knowledge_map`: domain docs, ADRs, architecture docs, diagrams, memory file, `.work/`
- `instructions_map`: instruction files with scope and globs
- `work_items`: provider, CLI, state names
- `gaps[]`: missing instructions/docs recommended by discovery

## 3. Agents

- `requirements-analyst`: Boards context -> `requirements.md` (skills: grill-me, create-prd)
- `planner`: requirements -> `plan.md` (skills: domain-modeling, backend-planning, ui-planning, test-planning). Gate: plan approval
- `implementer`: executes plan (skills: backend-development, ui-development, testing). Order: backend -> UI -> tests
- `reviewer`: read-only; build, tests, diff review -> `review.md`; ends in human triage
- Optional later: `pr-publisher`

## 4. Skills

- Foundation: `project-discovery`, `save-learning`, `update-documentation`
- Requirements: `grill-me`, `create-prd`
- Planning (read-only): `domain-modeling`, `backend-planning`, `ui-planning`, `test-planning` (may merge)
- Implementation: `backend-development`, `ui-development`, `testing`
- Possible: `bug-investigation`, `code-review-checklist`
- Common step in each layer skill: read profile + instruction file -> find reference feature -> act -> verify with profile commands -> report learnings
- Existing `backend-feature-slice` is the basis for `backend-development` (strip Units-specific parts)

## 5. Instruction files (rules live here)

- `copilot-instructions.md` (global, pointer to profile)
- `backend.instructions.md`
- `ui` / existing `blazor.instructions.md`
- `shared-contracts.instructions.md`
- `testing.instructions.md` (or split unit / integration / architecture)
- `documentation.instructions.md`
- `identity.instructions.md` (only if distinct rules)
- `workflow.instructions.md` (gates, front-matter, triage rules, `.work/` formats)

## 6. Scenarios (S = skill, A = agent, H = human gate)

| Case | Flow |
|---|---|
| First-time setup | [S project-discovery] -> [H approve profile and gaps] -> [S save-learning: draft instructions] -> [H approve] -> [S update-documentation: bootstrap docs] |
| Profile refresh | [S project-discovery: diff] -> [H approve] -> [S update-documentation] |
| New idea | [A requirements-analyst: grill-me] -> [S create-prd] -> [H approve] -> [S domain-modeling] -> [S update-documentation] -> [H approve] -> [Boards backlog items, approval] |
| Domain refactor | [S domain-modeling: current vs target] -> [S grill-me] -> [S create-prd] -> [H approve] -> [A planner: incremental tasks, migration] -> [H approve plan] -> per-task delivery flow -> [S update-documentation + ADR] |
| Full-stack feature | [Boards context, In Progress, H approve] -> [A requirements-analyst] -> [H approve requirements] -> [A planner] -> [H approve plan] -> [A implementer: backend -> UI -> testing] -> [A reviewer] -> [H triage] -> [A implementer: approved findings] -> [S testing] -> [S update-documentation] -> [S save-learning] -> [H approve] -> [PR, H approve] |
| Backend-only feature | [context] -> [S backend-planning] -> [H approve] -> [S backend-development] -> [S testing] -> [A reviewer] -> [H triage] -> [S update-documentation] -> [PR] |
| UI-only feature | [context] -> [lookup API contract via profile] -> [S ui-planning] -> [H approve] -> [S ui-development] -> [S testing] -> [A reviewer] -> [H triage] -> [PR] |
| Quick change (no agents) | [S backend-development / ui-development] -> [S testing] -> [S save-learning if surprising] -> [manual commit] |
| Small refactor | [layer skill: refactor mode] -> [S testing: existing tests pass] -> [S update-documentation if structure changed] |
| Add tests | [S test-planning: gaps] -> [S testing] -> [verify with profile commands] |
| Bug, clear repro | [context] -> [S testing: failing test] -> [layer skill: fix] -> [S testing: green] -> [A reviewer] -> [H triage] -> [S save-learning if undocumented convention] -> [PR] |
| Bug, unclear cause | [S grill-me] -> [read-only investigation, root cause note] -> [H confirm] -> clear-repro flow |
| Hotfix | [minimal context] -> [failing test] -> [fix] -> [build and tests] -> [H approve] -> [PR]; docs/learning deferred |
| Review only | [A reviewer: diff vs instructions and profile] -> [S testing: run] -> [review.md] -> [H triage] -> [S save-learning] |
| Documentation drift | [S project-discovery: diff] -> [S update-documentation] -> [H approve] |
| Knowledge cleanup | [S save-learning: review memory.md] -> [classify: rule / ADR / delete] -> [H approve] |
| Dependency upgrade | [layer skill: upgrade] -> [S testing: full suite] -> [A reviewer] -> [H triage] -> [S update-documentation if ADR affected] |

## 7. Review and triage (no automatic loop)

- Reviewer writes `review.md`: findings by severity (blocker, should-fix, nit, observation), each with ID, location, impact, fix-size estimate, suggested action; build/test results listed separately; acceptance criteria checked; learning candidates listed, not saved.
- Finding statuses: `open`, `fix`, `defer`, `reject` (with reason), `replan`, `learning`, `done`.
- Triage via chat, via editing the file, or both. Agent re-reads the file before acting, writes chat decisions back, and asks if chat and file conflict. `open` findings are never actioned.
- Implementer touches only approved finding IDs and reports other observations instead of fixing them.
- Re-review is optional and scoped to changed files and approved findings.
- Rejected findings are not re-raised.
- Docs and learning run after triage is finished.
- Same hybrid approach applies to other gates (requirements open questions, plan approval, save-learning proposals), defined once in `workflow.instructions.md`.

## 8. Issues found in the original proposal

- Folder layout assumes a standalone `workflow/` root and `docs/`, while this repo uses `.github` and `wiki/adr`, `wiki/domain`.
- Profile, instruction files and `copilot-instructions.md` can overlap (rule: profile = facts and pointers only).
- Clarify Engineering Memory (`memory.md`) vs Copilot native memory.
- No explicit test/QA step (build and tests must be mandatory in review).
- Plan approval was not a visible gate.
- Too many skills up front; start with the foundation set.
- Skill descriptions must have clear triggers to avoid wrong auto-selection.
- Skills must degrade gracefully when no profile exists (offer to run `project-discovery`).
- Verify precedence if a repo skill and a global skill share a name (relevant if skills go global later).

## 9. Open decisions

- Doc root: `wiki/` (current) or `docs/`
- Work item provider: Azure Boards or GitHub Issues (repo is on GitHub)
- One `testing.instructions.md` or three
- Planning skills separate, or folded into layer skills as plan mode
- Do hotfix and quick-change flows skip gates (lightweight mode)?
- Later: personal skills folder vs plugin for reuse and team sharing

## 10. Suggested build order

1. `project-discovery` + profile
2. `workflow.instructions.md` (gates, front-matter, triage)
3. `backend-development`, `ui-development`, `testing`
4. `save-learning`, `update-documentation`
5. Planning skills, `grill-me`, `create-prd`
6. Agents
7. Boards orchestrator (.NET file-based app, Azure CLI)
8. PR automation
