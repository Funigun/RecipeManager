---
status: draft
story: Documentation/ProductBacklog/UI/login-registration/User stories/sign-in.md
---
# UI plan: Sign in

## Sources
| Item | Origin | Why needed now |
|---|---|---|
| Shared dialog sign-in mode | `Documentation/ProductBacklog/UI/login-registration/User stories/choose-authentication-form.md`, AC-02 and AC-03: Sign in opens the shared dialog in sign-in mode and modes switch without leaving it. | Defines the entry and context for this workflow. |
| Regular-user default | `Documentation/ProductBacklog/UI/layouts/User stories/navigate-regular-user-features.md`, AC-03: after a regular user logs in, the default view is Dishes. | Required to implement the regular-user destination in AC-10. |
| Administrator default | `Documentation/ProductBacklog/UI/layouts/User stories/navigate-administration.md`, AC-05: administrator default is the admin overview and the existing admin sidebar remains available. | Required to implement the administrator destination in AC-10. |
| Credential and destination rules | `Documentation/ProductBacklog/UI/login-registration/prd.md`, R-02: username/password sign-in; R-05: regular users go to Dishes and administrators to Admin Panel overview. | Provides the sign-in input constraints and approved role-specific outcome. |

## Consumed API
| Endpoint (method + route) | Origin (contract / OpenAPI / code path) | DTOs | Used by |
|---|---|---|---|
| `POST /api/auth/login` | `RecipeManager.Identity.API/Features/Authorization/AuthGroup.cs` (`LogInRoute`) and `Features/Authorization/LogIn.cs`. | Request: `UserName`, `Password`. Response: `Token`, `RefreshToken`, `ExpirationDate`. JWT claims are created in `Features/Common/AuthorizationService.cs`, including `ClaimTypes.Role` (role values include `User` and `Admin`). | Sign-in workflow, session establishment, role-aware post-login navigation. |

## Backend gaps
- None identified for credential sign-in or role-specific destinations. The endpoint returns the tokens and expiry and its JWT includes role claims.

## Scope
Sign-in mode of the shared authentication dialog, a focused identity API client/workflow, client authentication state required to complete the sign-in, and navigation to existing `/dishes` or `/admin` pages. Existing `/login` page is not the entry flow. The UI project is Blazor WebAssembly and currently has no authentication-state provider, identity API client, or token handling; select a session-storage approach consistent with the app's security guidance rather than assuming one exists. Use the sign-in prototype only for form layout.

## File structure
```text
RecipeManager.UI/
├── Features/Account/Login/SignInView.razor [create] — sign-in workflow and result handling
├── Features/Account/Login/Components/SignInForm.razor [create] — username/password fields and callbacks
├── Features/Account/Login/Models/SignInFormModel.cs [create] — sign-in input model
├── Features/Account/Login/SignInApiClient.cs [create] — focused client for POST /api/auth/login
├── Features/Account/Shared/AuthenticationDialog.razor [modify] — render sign-in workflow in sign-in mode
├── Shared/Navigation/UiRoutes.cs [modify] — shared constants for existing /dishes and /admin routes
└── Program.cs [modify] — register the sign-in API client
```

## Tasks
| ID | Description | Files/area | Depends on | Covers (AC-/R-) |
|---|---|---|---|---|
| U-1 | Implement the sign-in form model and component for username/password input; keep HTTP orchestration out of the form. | `RecipeManager.UI/Features/Account/Login/Models/SignInFormModel.cs`, `RecipeManager.UI/Features/Account/Login/Components/SignInForm.razor` | Shared dialog from `choose-authentication-form` | AC-08 |
| U-2 | Add a focused client for `POST /api/auth/login`, map the request and response, and keep pending/failure state in the workflow. Resolve the API origin before configuring the client. | `RecipeManager.UI/Features/Account/Login/SignInApiClient.cs`, `RecipeManager.UI/Features/Account/Login/SignInView.razor`, `RecipeManager.UI/Program.cs` | U-1; Identity API origin decision | AC-09 |
| U-3 | Use the existing role claim to select Dishes or Admin Panel after sign-in. Do not implement this task until the token/session persistence and role-state policy is agreed; then reuse the existing route definitions through `UiRoutes`. | `RecipeManager.UI/Features/Account/Login/SignInView.razor`, `RecipeManager.UI/Shared/Navigation/UiRoutes.cs` | U-2; regular-user route values from `navigate-regular-user-features`; token/session policy | AC-10 |
| U-4 | Render the workflow in the shared dialog and keep the form responsive. Error presentation and resource-backed labels remain subject to the decisions below. | `RecipeManager.UI/Features/Account/Login/SignInView.razor`, `RecipeManager.UI/Features/Account/Shared/AuthenticationDialog.razor` | U-1–U-3 | AC-08, AC-09 |

## Test plan
| Level | What is tested | Covers | Task |
|---|---|---|---|
| Manual browser/UI | Username and password fields render and are submitted using the specified request field names. | AC-08 | U-1 |
| Manual network/contract | Valid credentials call `POST /api/auth/login`; verify the returned token fields without choosing a persistence policy. | AC-09 | U-2 |
| Manual browser/UI | After the token/session policy is resolved, successful `User` sign-in navigates to `/dishes` and `Admin` sign-in to `/admin`. | AC-10 | U-3 |
| Manual browser/network | Failed requests do not create an authenticated UI state; submitting state resets and entered credentials are retained as appropriate. | AC-09 | U-2, U-4 |
| Manual responsive | Sign-in form is usable at phone and desktop widths. | AC-08–AC-10 | U-4 |

## Risks and open questions
- **Unresolved product question:** the user-visible presentation of failed sign-in and unexpected request errors is not established by the PRD or target Story. Keep this decision open; do not choose inline, summary, toast, or other presentation silently.
- The UI currently has no token/session persistence or refresh behavior. Decide and document the client-side token handling policy before implementing authenticated UI state or AC-10; do not invent a backend contract. The Identity API's refresh endpoint is outside this Story's consumed API.
- `Program.cs` currently gives `HttpClient` the UI's base address. Confirm how the Aspire gateway exposes the Identity API to the browser before setting the sign-in client's base address.
- The UI has no localization resources or localization registration, while the Blazor instructions require resource-backed application text. Decide the resource setup before adding localized labels and error messages.
