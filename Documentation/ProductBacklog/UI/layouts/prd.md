---
status: approved
topic: logged-in-user-layouts
source: decisions.md
---
# Logged-in User Layouts

## 1. Summary

Recipe Manager needs distinct navigation experiences for visitors, regular signed-in users, and administrators. The layouts will expose each audience's relevant destinations and send signed-in users to their role-specific default view.

## 2. Goals and non-goals

- **Goals:** Define public, regular-user, and admin navigation; establish signed-in default destinations; provide profile and logout access.
- **Non-goals:** No additional non-goals were specified.

## 3. Users and permissions

| Role | Can | Cannot |
|------|-----|--------|
| Visitor | Use Home and Features navigation, and open sign-in or registration from the landing-page actions. | Not specified in this PRD. |
| Regular user | Navigate to Dishes, Recipes, Ingredients, Cookbooks, Meal Planner, and Profile; end the signed-in session. | Not specified in this PRD. |
| Admin user | Use regular-user destinations, open Admin Panel, navigate admin pages, and end the signed-in session. | Not specified in this PRD. |

## 4. Domain rules

### Layout structure and navigation

- **R-01 — Audience-specific layouts** *(Entities: Visitor, Regular user, Admin user; Source: D-01)*
  - Provide a base layout for the landing page, a user layout for regular signed-in users, and an admin layout for administrators.
- **R-02 — Regular-user navigation and default view** *(Entities: Regular user; Source: D-02, D-05)*
  - The signed-in menu includes Dishes, Recipes, Ingredients, Cookbooks, Meal Planner, and Profile.
  - Dishes is the default view after a regular user logs in.
- **R-03 — Admin navigation and default view** *(Entities: Admin user; Source: D-03, D-04, D-05)*
  - The admin menu includes all regular-user menu items plus Admin Panel.
  - Admin Panel opens the admin overview and is the default view after an administrator logs in.
  - The existing admin sidebar remains available for navigating administration pages.
- **R-04 — Public navigation** *(Assumption; Entities: Visitor; Source: decisions.md Assumptions)*
  - The base layout provides Home and Features navigation. Sign in and Register are available from the landing-page actions, which open the shared authentication dialog; the public header does not include separate authentication links.

## 5. User stories

### US-01 — Navigate public pages

As a visitor, I want to use the public navigation and landing-page account actions so that I can explore the landing page or open sign-in and registration.

**Acceptance criteria**
- **AC-01:** The base layout presents Home and Features navigation; the landing page's Sign in and Register actions open the shared authentication dialog in the corresponding form mode.

**Related domain rules:** R-01, R-04

### US-02 — Navigate regular-user features

As a regular user, I want the signed-in menu to expose my main Recipe Manager areas so that I can reach them from the user layout.

**Acceptance criteria**
- **AC-02:** The user layout presents Dishes, Recipes, Ingredients, Cookbooks, Meal Planner, and Profile in its top menu.
- **AC-03:** After a regular user logs in, the default view is Dishes.

**Related domain rules:** R-01, R-02

### US-03 — Navigate administration

As an admin user, I want access to both regular-user areas and administration so that I can manage the application from the admin layout.

**Acceptance criteria**
- **AC-04:** The admin top menu presents all regular-user menu items plus Admin Panel.
- **AC-05:** After an administrator logs in, the default view is the admin overview; the existing admin sidebar remains available for administration-page navigation.

**Related domain rules:** R-02, R-03

### US-04 — End a signed-in session

As a signed-in user, I want a Logout button so that I can end my session.

**Acceptance criteria**
- **AC-06:** The user and admin layouts each present a Logout button, and selecting it ends the signed-in session.

**Related domain rules:** R-02, R-03

### Product constraints (not user stories)

- None specified.

## 6. UI and UX needs

The base layout serves the public landing experience with Home and Features navigation. Its Sign in and Register actions open the shared authentication dialog. Signed-in regular users receive the feature navigation and Profile; administrators receive the same navigation plus Admin Panel and retain the administration sidebar. Both signed-in layouts include Logout. See AC-01 through AC-06 for observable behavior.

## 7. Non-functional requirements

No non-functional requirements were specified.

## 8. Impact on existing system

The current UI uses one route-sensitive layout to switch between public and signed-in navigation. Its signed-in menu currently includes Dishes, Recipes, Cookbooks, Meal plans, and Profile; it does not include Ingredients or Logout. Administration currently uses a sidebar, which this PRD retains, but the menu does not include an Admin Panel entry. The requested layouts and menu behavior therefore require changes to the existing navigation and role-specific default views. No data migration is identified for this navigation-focused context.

## 9. Open questions and assumptions

- **Assumption:** The base layout retains Home and Features navigation. Sign in and Register remain landing-page actions that open the shared authentication dialog, as defined in the login-registration context.
- No open questions remain from the approved decisions.

## 10. Documentation targets

No `wiki/domain` or `wiki/adr` updates are identified; the approved decisions concern UI navigation and layouts and introduce no domain-model change or architectural decision.
