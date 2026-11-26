---
status: ready
depends_on: []
---
# End a signed-in session

As a signed-in user, I want a Logout button so that I can end my session.

## Preconditions

- The user is signed in.

## Successful flow

1. The user selects Logout from the user or admin layout.
2. The signed-in session ends.

## State changes

- The user transitions from a signed-in to a signed-out state.

## Acceptance criteria

- **AC-06:** The user and admin layouts each present a Logout button, and selecting it ends the signed-in session.

## Traceability

- Acceptance criteria: AC-06
- Domain rules: R-02, R-03
