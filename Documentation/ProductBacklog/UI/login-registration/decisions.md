---
status: approved
topic: Login and registration dialog
---
## Table of contents

- [Authentication entry](#authentication-entry)

## Decisions

### Authentication entry

- <u>D-01 — Landing-page dialog entry (User)</u>

  **Decision:** The landing page's login and registration buttons open the same dialog, with the corresponding form selected. Remove the separate login and registration pages.

  **Reason:** The user wants one shared dialog instead of dedicated pages for each form.

- <u>D-02 — Public navigation entry points (User)</u>

  **Decision:** Remove the public header's Sign in and Register links. The landing-page buttons are the only entry points for the authentication dialog.

  **Reason:** The user chose to keep the authentication dialog entry on the landing page only.

- <u>D-03 — Registration success (Recommended)</u>

  **Decision:** After successful registration, show a success message and switch the dialog to the Sign in form.

  **Reason:** The existing registration endpoint creates the account but does not issue an authentication token, so the user must sign in separately.

- <u>D-04 — Password confirmation (Recommended)</u>

  **Decision:** Include a confirm-password field in the registration form and require it to match the password before submission. Do not send the confirmation field to the API.

  **Reason:** This catches password-entry mistakes before account creation while keeping the existing API contract unchanged.

- <u>D-05 — Password recovery scope (Recommended)</u>

  **Decision:** Password recovery is out of scope for this login and registration UI.

  **Reason:** The current API has no password-reset flow, and the user chose to keep the work focused on login and registration.

- <u>D-06 — Existing authentication routes (Recommended)</u>

  **Decision:** Remove the `/login` and `/register` routes. Direct visits to those URLs are no longer supported; the landing-page buttons are the only way to open the dialog.

  **Reason:** The user chose to remove the pages and keep authentication entry on the landing page.

- <u>D-07 — Role-specific post-sign-in destinations (Derived from approved layouts)</u>

  **Decision:** After successful sign-in, regular users go to Dishes and administrators go to the Admin Panel overview.

  **Reason:** These are the approved default destinations in the logged-in-user-layouts context.

## Open questions

## Assumptions

- The forms follow the existing identity API: sign-in requires username and password; registration requires username, email, and password. The backend requires usernames of at most 15 characters, valid email addresses, and passwords of at least 10 characters containing lowercase, uppercase, numeric, and special characters.

## Out of scope

- Defining the Regular User and Admin layouts is covered by the logged-in-user-layouts context.

## Glossary
