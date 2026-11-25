---
status: approved
topic: Identity management
source: decisions.md
---
# Identity Self-Service Account Management

## 1. Summary
This slice prioritizes self-service identity and security controls for account holders. It enables users to change their sign-in email after verification, recover a forgotten password, and change their password, while keeping account administration and app-specific preferences out of scope.

## 2. Goals and non-goals
- Goals:
  - Enable account holders to update their sign-in email without switching away from the current address before the new one is verified.
  - Enable account holders to recover access and change their password without administrator assistance.
  - Preserve the existing Regular/Admin role model and registration behavior.
- Non-goals:
  - Admin management of user accounts, roles, or permissions.
  - Changes to cooking or meal-planning preferences.
  - Self-service account closure or personal-data deletion.
  - Blazor UI design or implementation; this is deferred to the UI refactor.

## 3. Users and permissions
| Role | Can | Cannot |
|------|-----|--------|
| Account holders (Regular and Admin) | <ul><li>Change the sign-in email and password for their own account.</li><li>Recover access to their own account.</li></ul> | <ul><li>Manage another user's account.</li><li>Manage roles or permissions as part of this slice.</li></ul> |

## 4. Domain rules

### Scope and account ownership

- **R-01 — Self-service account ownership** *(Entities: Account holder, Account; Source: Assumption)*
  - Self-service account operations apply only to the authenticated account holder's own account.
- **R-06 — Existing roles and admin management** *(Entities: Account, Role; Source: D-09)*
  - The existing Regular/Admin role model remains unchanged.
  - Admin user, role, and permission management is outside this slice.
- **R-07 — Account closure and data deletion** *(Entities: Account; Source: D-06)*
  - Account closure and personal-data deletion are outside this slice.
- **R-08 — App-specific preferences** *(Entities: Account; Source: D-03)*
  - Cooking and meal-planning preferences are outside the identity and security account area.
- **R-09 — Existing registration behavior** *(Entities: Account; Source: Assumption)*
  - Existing registration behavior remains in place.

### Sign-in email

- **R-02 — Verified sign-in email change** *(Entities: Account, sign-in email; Source: D-04)*
  - A sign-in email change takes effect only after the new address is verified.
- **R-03 — Current email remains active while pending** *(Entities: Account, sign-in email; Source: D-08)*
  - The current sign-in email remains active until the new address is verified.

### Password and recovery

- **R-04 — Self-service password recovery** *(Entities: Account, password; Source: D-05)*
  - Account holders can recover access through a self-service password-recovery flow.
- **R-05 — Authenticated password change** *(Entities: Account, password; Source: D-10)*
  - Authenticated account holders can change their own password.

## 5. User stories

Each story states a user outcome and its agreed acceptance criteria. Related domain rules are referenced rather than repeated.

### US-01 — Change sign-in email
As an account holder, I want to change my sign-in email after verifying the new address so that I can keep my account details current without losing access while the change is pending.

**Acceptance criteria**
- **AC-01:** Given an account holder has requested a sign-in email change and has not verified the new address, when they continue to use their account, the current sign-in email remains active.
- **AC-02:** Given an account holder has verified the new address, when the email change is completed, the new address is used as the sign-in email.

**Related domain rules:** R-01, R-02, R-03

### US-02 — Recover account access
As an account holder who cannot sign in, I want to recover access myself so that I can regain use of my account without manual assistance.

**Acceptance criteria**
- **AC-03:** Given an account holder cannot sign in, when they complete the password-recovery flow, they can regain access using a new password.

**Related domain rules:** R-01, R-04

### US-03 — Change password
As an authenticated account holder, I want to change my password so that I can maintain control of my account's security.

**Acceptance criteria**
- **AC-04:** Given an authenticated account holder changes their password, the new password is accepted for subsequent sign-in to that account.

**Related domain rules:** R-01, R-05

### Product constraints (not user stories)
- Preserve the existing Regular/Admin roles and registration behavior; this slice excludes admin account management, account closure, personal-data deletion, and cooking or meal-planning preferences.

**Related domain rules:** R-06, R-07, R-08, R-09

## 6. UI and UX needs
Blazor UI design and implementation are deferred to the later UI refactor. This PRD defines account-flow outcomes only and does not specify screens, interaction details, or loading, empty, and error states.

## 7. Non-functional requirements
Email delivery, verification and recovery token lifecycle, and detailed security controls remain to be specified during implementation design. No performance, availability, hosting, or data-retention requirements were established by the decisions.

## 8. Impact on existing system
The Identity API already supports registration and sign-in. Its password-change handler is currently a placeholder, so this slice must provide the password-change behavior described above. Sign-in email change and password recovery are additional account capabilities. The existing Regular/Admin role model remains in place; admin account management, account closure, and personal-data deletion are not changed by this slice.

## 9. Open questions and assumptions
- Email delivery, verification and recovery token lifecycle, and detailed security controls are implementation-design questions; the approved decisions define the user outcomes but not these mechanisms.
- Self-service account operations apply to the authenticated user's own account.
- No migration or breaking-change requirement has been decided.

## 10. Documentation targets
- Create `wiki/domain/identity.md` to document the account, sign-in email, and password-recovery domain concepts and their agreed rules.
- Update or create an ADR only if implementation design introduces an architectural decision that warrants one.
