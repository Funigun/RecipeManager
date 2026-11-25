---
status: approved
topic: Identity management
---

## Table of contents

- [Scope and account ownership](#scope-and-account-ownership)
- [Sign-in email](#sign-in-email)
- [Password and recovery](#password-and-recovery)

## Decisions

### Scope and account ownership

- <u>D-01 — Work ID (User)</u>

  **Decision:** Use `identity-management` as the work ID.

  **Reason:** Provides a broad location for decisions about account management and administration.

- <u>D-02 — Initial priority (User)</u>

  **Decision:** Prioritize user self-service account management; defer admin user-management features.

  **Reason:** The user chose to focus on account settings rather than the more complex admin panel.

- <u>D-03 — Account-area scope (User)</u>

  **Decision:** Focus on identity and security; exclude cooking and meal-planning preferences.

  **Reason:** The user chose a focused account area rather than app-specific settings.

- <u>D-06 — Account closure (User)</u>

  **Decision:** Defer self-service account closure and data deletion from this slice.

  **Reason:** The user chose to keep data-lifecycle decisions separate from account access and security.

- <u>D-07 — UI scope (User)</u>

  **Decision:** Defer Blazor UI design and implementation to the later UI refactor; focus this decision set on Identity API account flows.

  **Reason:** The user said Blazor UI is a separate refactoring topic to design later.

- <u>D-09 — Admin user management (User)</u>

  **Decision:** Defer the admin panel for managing user accounts, roles, and permissions; retain the existing Regular/Admin role model for now.

  **Reason:** The user chose user self-service as the priority; existing project guidance already defines the two roles.

### Sign-in email

- <u>D-04 — Sign-in email changes (User)</u>

  **Decision:** Allow users to change their sign-in email after verifying the new address.

  **Reason:** The user chose verified email changes as part of self-service account management.

- <u>D-08 — Email-change activation (User)</u>

  **Decision:** Keep the current sign-in email active until the new email address is verified.

  **Reason:** The user chose option A to preserve access while the new address is pending.

### Password and recovery

- <u>D-05 — Password recovery (User)</u>

  **Decision:** Include a self-service password-recovery flow.

  **Reason:** The user chose to let locked-out users recover access without manual assistance.

- <u>D-10 — Password change (User)</u>

  **Decision:** Include authenticated password change in this slice, replacing the current placeholder.

  **Reason:** The user selected inclusion after being informed the current handler is a placeholder.

## Open questions

## Assumptions
- Self-service endpoints operate on the authenticated user's own account.
- Existing registration behavior remains in place; password-change scope is defined by Decision 10.
- Email delivery, token lifecycle, and security details will be specified during PRD and implementation design.

## Out of scope
- Admin panel for user account, role, or permission management.
- Changes to the existing Regular/Admin role model.
- Cooking and meal-planning preferences.
- Self-service account closure and personal-data deletion.
- Blazor UI design and implementation; handled later as part of the UI refactor.

## Glossary
- **Sign-in email**: The email address associated with the user's account and used for sign-in and recovery.
