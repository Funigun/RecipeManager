---
status: approved
topic: Landing page
source: decisions.md
---
# Landing page

## 1. Summary

Replace the current welcome page with a landing page that introduces Recipe Manager and offers visitors sign-in and registration entry points. Keep the feature areas visible by name, while leaving their content and component shapes as placeholders.

## 2. Goals and non-goals

- **Goals**
  - Explain the purpose of Recipe Manager on the landing page.
  - Provide access to sign-in and registration through the shared authentication dialog.
  - Show the named feature areas without defining their detailed content or component designs.
- **Non-goals**
  - Add or change recipe-management, account, or meal-planning capabilities.
  - Define the detailed design or content of individual feature components.
  - Add an About page or About section.

## 3. Users and permissions

| Role | Can | Cannot |
|------|-----|--------|
| Visitor | View the landing page and use its account actions. | No additional restrictions are defined for this page. |

## 4. Domain rules

This presentation change introduces no domain rules.

## 5. User stories

### US-01 — Explore the landing page

As a visitor, I want to understand what Recipe Manager offers and find account entry points so that I can decide how to proceed.

**Acceptance criteria**

- **AC-01:** Given a visitor opens `/`, the page presents the Recipe Manager hero message and Sign in and Register actions with the hero; the public header has no authentication links.
- **AC-02:** Selecting Sign in or Register opens the shared authentication dialog in the corresponding form mode; it does not navigate to a separate `/login` or `/register` page.
- **AC-03:** The page shows labeled sections for Dishes, Recipes, Cookbooks, Meal Plan, and shared units and categories, with placeholder content rather than defined component designs.
- **AC-04:** The Features navigation takes visitors to the feature sections.

**Related domain rules:** None.

### Product constraints (not user stories)

- The About us navigation link and a separate About page are out of scope. *(D-04)*
- The feature-section labels remain, while their content and component shapes are deferred. *(D-01, D-02)*

## 6. UI and UX needs

The `/` landing page keeps its hero message, account actions, and Features navigation. The navigation leads to the named feature sections, which use placeholders while detailed component designs are deferred. Sign in and Register open the shared authentication dialog in the corresponding form mode. The About us link is omitted.

## 7. Non-functional requirements

None specified.

## 8. Impact on existing system

The existing welcome experience at `/` is replaced. Sign-in and registration actions open the shared authentication dialog defined in the login-registration context. No backend behavior, domain data, or other application capabilities are changed.

## 9. Open questions and assumptions

- The content and component shapes for the feature-section placeholders are intentionally deferred.

## 10. Documentation targets

No `wiki/domain` or `wiki/adr` updates are identified; this change adds no domain model or architectural decision.
