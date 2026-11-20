# Identity domain

## Purpose
An account holder manages their own sign-in email and password, including recovering access without administrator assistance. The existing registration behavior and Regular/Admin role model remain unchanged.

## Entities and value objects
| Name | Kind (entity/value object) | Description | Key attributes |
|------|----------------------------|-------------|----------------|
| Account (`User`) | Entity | Existing identity account. **Changed:** supports self-service sign-in email and password management; registration behavior is retained. | Account holder, active sign-in email, password credential, existing role |
| Sign-in email | Value object | The email address used to sign in. **Changed:** a requested replacement is pending until verified; the current address remains active until then. | Address, verification status |
| Password credential | Value object | The account's password credential, which can be changed by its authenticated holder or replaced through account recovery. | Password |
| Account role (`Role`) | Entity | Existing Regular/Admin role model, retained without role or permission management in this slice. | Role |

## Relationships
- An account holder operates on their own account only.
- An account has one active sign-in email; while a replacement is unverified, the existing email remains active. After verification, the new email becomes the sign-in email.
- An account has a password credential that can be changed by its authenticated holder or reset through self-service recovery.
- An account retains its existing role; this slice does not add account administration or role/permission management.

## Invariants and rules
| Rule | Applies to | PRD ref |
|------|------------|---------|
| Self-service operations apply only to the authenticated account holder's own account. | Account | R-01 |
| A requested sign-in email change takes effect only after the new address is verified. | Account, sign-in email | R-02 |
| The current sign-in email remains active until the replacement is verified. | Account, sign-in email | R-03 |
| An account holder can recover access through a self-service password-recovery flow. | Account, password credential | R-04 |
| An authenticated account holder can change their own password. | Account, password credential | R-05 |
| The Regular/Admin role model remains unchanged; admin management of users, roles, and permissions is outside this slice. | Account, account role | R-06 |
| Account closure and personal-data deletion are outside this slice. | Account | R-07 |
| Cooking and meal-planning preferences are outside the identity and security account area. | Account | R-08 |
| Existing registration behavior remains in place. | Account | R-09 |

## Lifecycle / states
- **Sign-in email:** current address remains active while a replacement is pending verification; successful verification makes the replacement active.
- **Password recovery:** a self-service flow lets an account holder regain access with a new password. The PRD does not define the recovery-token lifecycle.

## Open questions
- Email delivery, verification and recovery token lifecycles, and detailed security controls remain unspecified in the approved PRD and are implementation-design questions.
