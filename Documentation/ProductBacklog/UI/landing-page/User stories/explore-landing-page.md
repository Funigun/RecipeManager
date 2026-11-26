---
status: draft
depends_on:
  - Documentation/ProductBacklog/UI/login-registration/User stories/choose-authentication-form.md
---
# Explore the Recipe Manager landing page

As a visitor, I want to understand what Recipe Manager offers and find account entry points so that I can decide how to proceed.

## Behavior

- The `/` page presents the Recipe Manager hero message and Sign in and Register actions with the hero; the public header has no authentication links.
- Sign in and Register open the shared authentication dialog in the corresponding form mode; they do not navigate to separate `/login` or `/register` pages.
- The page shows labeled sections for Dishes, Recipes, Cookbooks, Meal Plan, and shared units and categories, with placeholder content.
- The Features navigation takes visitors to the feature sections.

## Acceptance criteria

- **AC-01:** Given a visitor opens `/`, the page presents the Recipe Manager hero message and Sign in and Register actions with the hero; the public header has no authentication links.
- **AC-02:** Selecting Sign in or Register opens the shared authentication dialog in the corresponding form mode.
- **AC-03:** The page shows labeled sections for Dishes, Recipes, Cookbooks, Meal Plan, and shared units and categories, with placeholder content rather than defined component designs.
- **AC-04:** The Features navigation takes visitors to the feature sections.

## Traceability

- Acceptance criteria: AC-01, AC-02, AC-03, AC-04
- Domain rules: None

## Open questions

- The content and component shapes for the feature-section placeholders are intentionally deferred.
