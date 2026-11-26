---
status: ready
depends_on:
  - Documentation/ProductBacklog/UI/login-registration/User stories/choose-authentication-form.md
  - Documentation/ProductBacklog/UI/layouts/User stories/navigate-regular-user-features.md
  - Documentation/ProductBacklog/UI/layouts/User stories/navigate-administration.md
---
# Sign in

As a returning user, I want to sign in with my credentials so that I can access my account.

## Preconditions

- The shared authentication dialog is open in sign-in mode.

## Successful flow

1. The visitor enters a username and password.
2. When valid credentials are submitted, sign-in succeeds.
3. A regular user is taken to Dishes; an administrator is taken to the Admin Panel overview.

## Acceptance criteria

- **AC-08:** The sign-in form accepts the username and password defined by R-02.
- **AC-09:** When valid credentials are submitted, the sign-in operation succeeds.
- **AC-10:** After successful sign-in, a regular user is taken to Dishes and an administrator is taken to the Admin Panel overview.

## Traceability

- Acceptance criteria: AC-08, AC-09, AC-10
- Domain rules: R-02, R-05

## Open questions

- The user-visible presentation of failed sign-in and unexpected request errors is not established by the PRD.
