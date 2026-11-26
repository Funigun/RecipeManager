---
status: ready
depends_on:
  - Documentation/ProductBacklog/UI/login-registration/User stories/choose-authentication-form.md
---
# Create an account

As a public visitor, I want to register with my account details so that I can use Recipe Manager.

## Preconditions

- The shared authentication dialog is open in registration mode.

## Successful flow

1. The visitor enters a username, email, password, and confirmation password.
2. The dialog submits the account details without sending the confirmation password.
3. After successful registration, the dialog displays a success message and switches to the Sign in form.

## Alternate and failure outcomes

- Registration cannot be submitted when the confirmation password does not match the password.

## State changes

- Successful registration does not sign the visitor in; the dialog switches to Sign in.

## Acceptance criteria

- **AC-05:** The registration form accepts the username, email, password, and confirmation password defined by R-02 and R-03.
- **AC-06:** Registration cannot be submitted when the confirmation password does not match the password.
- **AC-07:** After successful registration, the dialog displays a success message and switches to the Sign in form.

## Traceability

- Acceptance criteria: AC-05, AC-06, AC-07
- Domain rules: R-02, R-03, R-04

## Open questions

- The user-visible presentation of registration validation and unexpected request errors is not established by the PRD.
