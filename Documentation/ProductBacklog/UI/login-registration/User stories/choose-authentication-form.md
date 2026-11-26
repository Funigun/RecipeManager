---
status: ready
depends_on: []
---
# Choose an authentication form

As a public visitor, I want to open and switch between sign-in and registration in one dialog so that I can choose the account action I need.

## Behavior

- Selecting Register on the landing page opens the shared dialog in registration mode.
- Selecting Sign in on the landing page opens the same dialog in sign-in mode.
- The visitor can switch between registration and sign-in without leaving the dialog.
- The public header has no Sign in or Register links, and direct visits to `/login` or `/register` do not open the dialog.

## Acceptance criteria

- **AC-01:** When the visitor selects Register on the landing page, the shared dialog opens in registration mode.
- **AC-02:** When the visitor selects Sign in on the landing page, the same dialog opens in sign-in mode.
- **AC-03:** The visitor can switch between registration and sign-in without leaving the dialog.
- **AC-04:** The public header has no Sign in or Register links, and direct visits to `/login` or `/register` do not open the authentication dialog.

## Traceability

- Acceptance criteria: AC-01, AC-02, AC-03, AC-04
- Domain rules: R-01
