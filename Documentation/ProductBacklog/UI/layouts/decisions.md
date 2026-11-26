---
status: approved
topic: logged-in-user-layouts
---
## Table of contents

- [Layout structure and navigation](#layout-structure-and-navigation)

## Decisions

### Layout structure and navigation

- <u>D-01 — Provide three layout variants (User)</u>

  **Decision:** Define a base layout for the landing page, a user layout for regular signed-in users, and an admin layout for administrators.

  **Reason:** The layouts distinguish public, regular-user, and administrator navigation.

- <u>D-02 — User navigation and landing destination (User)</u>

  **Decision:** The user layout's top menu contains Dishes, Recipes, Ingredients, Cookbooks, and Meal Planner; Dishes is the default view after login.

  **Reason:** These are the specified primary destinations for regular users.

- <u>D-03 — Admin navigation and landing destination (User)</u>

  **Decision:** The admin layout has the same top-menu items as the user layout, plus Admin Panel; Admin Panel is the default view after login.

  **Reason:** Administrators retain regular-user destinations and land in administration.

- <u>D-04 — Keep admin sidebar navigation (User)</u>

  **Decision:** The Admin Panel top-menu entry opens the admin overview, and the existing admin sidebar remains for navigating administration pages.

  **Reason:** The sidebar preserves direct navigation to the existing admin catalog pages.

- <u>D-05 — Keep Profile and add Logout (User)</u>

  **Decision:** Include Profile and a Logout button in the signed-in navigation for both regular users and administrators, in addition to the specified feature links.

  **Reason:** Preserve access to the existing profile page and provide a way to end a signed-in session.

## Open questions

## Assumptions

- The base layout retains public Home and Features navigation. Sign in and Register are available from the landing-page actions, which open the shared authentication dialog defined in the login-registration context.

## Out of scope

## Glossary
