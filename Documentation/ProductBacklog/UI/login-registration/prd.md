---
status: approved
topic: Login and registration dialog
source: decisions.md
---
# Login and Registration Dialog

## 1. Summary

Visitors need a clear way to create an account or sign in to Recipe Manager. This PRD defines one shared authentication dialog opened from the landing page. After successful sign-in, regular users go to Dishes and administrators go to the Admin Panel overview, as defined by the logged-in-user-layouts context.

## 2. Goals and non-goals

- Goals:
  - Let visitors open registration or sign-in in the same dialog from the landing page.
  - Support the existing account registration and sign-in inputs.
  - Guide users from successful registration to sign-in.
- Non-goals:
  - Password recovery.
  - Defining the Regular User or Admin layouts, which are specified in the logged-in-user-layouts context.

## 3. Users and permissions

| Role | Can | Cannot |
|------|-----|--------|
| Public visitor | Open the authentication dialog, register, and sign in | No additional restrictions are defined by this slice |

## 4. Domain rules

### Authentication

- **R-01 — Shared authentication entry** *(Entities: Public visitor, authentication dialog; Source: D-01, D-02, D-06)*
  - The landing-page Register and Sign in actions open the same dialog in the corresponding form mode. The visitor can switch between forms within the dialog.
  - The separate `/login` and `/register` routes and public-header authentication links are removed. Direct visits to those URLs are unsupported.
- **R-02 — Authentication inputs** *(Entities: Credentials; Source: approved assumption A-01 in decisions.md)* *(Assumption)*
  - Sign-in uses username and password; registration uses username, email, and password.
  - Follow the existing API constraints: username is at most 15 characters, email is valid, and password is at least 10 characters and contains lowercase, uppercase, numeric, and special characters.
- **R-03 — Registration confirmation** *(Entities: Registration form; Source: D-04)*
  - Registration requires a confirmation password that matches the password before submission. The confirmation value is not part of the registration request.
- **R-04 — Registration success** *(Entities: Account, authentication dialog; Source: D-03)*
  - After successful registration, show a success message and switch the dialog to the Sign in form. Registration does not sign the visitor in.
- **R-05 — Role-specific sign-in destination** *(Entities: Regular user, Admin user; Source: D-07 and logged-in-user-layouts PRD AC-03 and AC-05)*
  - After successful sign-in, navigate a regular user to Dishes and an administrator to the Admin Panel overview.

## 5. User stories

### US-01 — Choose an authentication form

As a public visitor, I want to open and switch between sign-in and registration in one dialog so that I can choose the account action I need.

**Acceptance criteria**

- **AC-01:** When the visitor selects Register on the landing page, the shared dialog opens in registration mode.
- **AC-02:** When the visitor selects Sign in on the landing page, the same dialog opens in sign-in mode.
- **AC-03:** The visitor can switch between registration and sign-in without leaving the dialog.
- **AC-04:** The public header has no Sign in or Register links, and direct visits to `/login` or `/register` do not open the authentication dialog.

**Related domain rules:** R-01

### US-02 — Create an account

As a public visitor, I want to register with my account details so that I can use Recipe Manager.

**Acceptance criteria**

- **AC-05:** The registration form accepts the username, email, password, and confirmation password defined by R-02 and R-03.
- **AC-06:** Registration cannot be submitted when the confirmation password does not match the password.
- **AC-07:** After successful registration, the dialog displays a success message and switches to the Sign in form.

**Related domain rules:** R-02, R-03, R-04

### US-03 — Sign in

As a returning user, I want to sign in with my credentials so that I can access my account.

**Acceptance criteria**

- **AC-08:** The sign-in form accepts the username and password defined by R-02.
- **AC-09:** When valid credentials are submitted, the sign-in operation succeeds.
- **AC-10:** After successful sign-in, a regular user is taken to Dishes and an administrator is taken to the Admin Panel overview.

**Related domain rules:** R-02, R-05

### Product constraints (not user stories)

- Password recovery is out of scope because the current API has no password-reset flow. *(Source: D-05)*
- The Regular User and Admin layouts are defined in the logged-in-user-layouts context; post-sign-in destinations follow its role-specific defaults.

## 6. UI and UX needs

The landing page provides the only entry points to the shared authentication dialog. The dialog supports registration and sign-in modes and allows the visitor to switch between them. Successful registration displays confirmation and moves to sign-in. After successful sign-in, regular users go to Dishes and administrators go to the Admin Panel overview, as defined in the logged-in-user-layouts context.

The [sign-in mockup](ui-prototype/login.html) and [registration mockup](ui-prototype/register.html) are visual references for form layout only. They do not define separate routes or override the shared-dialog behavior and role-specific destinations in this PRD.

## 7. Non-functional requirements

No additional non-functional requirements were established in the approved decisions.

## 8. Impact on existing system

- The landing-page actions currently navigate to separate login and registration pages; they will instead open the shared dialog in the matching mode.
- The public header currently includes Sign in and Register links, and the UI has separate `/login` and `/register` pages. These entry points and routes are removed.
- The existing identity API continues to provide the account registration and sign-in behavior. The confirmation-password value is UI-only and does not change the API contract.
- Registration currently creates an account without signing the user in, so the dialog must direct the user to sign in after successful registration.
- No data migration is identified for this UI change.

## 9. Open questions and assumptions

### Open questions

- The user-visible presentation of failed sign-in, registration validation, and unexpected request errors is not established by the decisions.

### Assumptions

- **A-01:** The forms follow the existing identity API: sign-in uses username and password; registration uses username, email, and password. Username length, email format, and password-composition constraints follow the existing API validation rules.

## 10. Documentation targets

No domain model or ADR documentation change is identified for this UI-only slice.
