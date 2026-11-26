---
status: draft
story: Documentation/ProductBacklog/UI/layouts/User stories/end-signed-in-session.md
---
# UI plan: End a signed-in session

## Sources
| Item | Origin | Why needed now |
|---|---|---|
| Successful sign-in/session establishment | `Documentation/ProductBacklog/UI/login-registration/User stories/sign-in.md`, AC-09: valid credentials result in successful sign-in. | Logout operates on the authenticated session established by this workflow. |
| Both layout audiences | `Documentation/ProductBacklog/UI/layouts/User stories/navigate-regular-user-features.md`, AC-02: regular-user layout exposes its signed-in feature menu; `navigate-administration.md`, AC-04: admin menu includes regular destinations plus Admin Panel. | Confirms Logout must be available in both signed-in navigation experiences. |
| Authenticated logout endpoint | `RecipeManager.Identity.API/Features/Authorization/LogOut.cs`: `POST /api/auth/logout` requires authorization and deletes the current user's refresh-token records. | Establishes the existing backend operation and bearer-auth requirement for logout. |

## Consumed API
| Endpoint (method + route) | Origin (contract / OpenAPI / code path) | DTOs | Used by |
|---|---|---|---|
| `POST /api/auth/logout` | `RecipeManager.Identity.API/Features/Authorization/AuthGroup.cs` (`LogOutRoute`) and `Features/Authorization/LogOut.cs`; `.RequireAuthorization()`, handler returns success after deleting current user's refresh tokens. | No request DTO/body; authenticated bearer token is required; success response has no body. | Logout action in both regular-user and admin layouts. |

## Backend gaps
- None: an authenticated logout endpoint exists.

## Scope
Logout control in the signed-in user and admin navigation, client auth/session state cleanup, and a focused auth client calling the existing logout endpoint. The current Blazor WebAssembly UI has no auth provider, token management, or logout control.

## File structure
```text
RecipeManager.UI/
├── Features/Account/Logout/LogoutView.razor [create] — logout workflow and API result handling
├── Features/Account/Logout/LogoutApiClient.cs [create] — focused client for POST /api/auth/logout
├── Layout/MainLayout.razor [modify] — Logout action in both signed-in menus
└── Program.cs [modify] — register the logout API client
```

## Tasks
| ID | Description | Files/area | Depends on | Covers (AC-/R-) |
|---|---|---|---|---|
| U-1 | Add a focused authenticated client for `POST /api/auth/logout`; send no body and use the established session token. | `RecipeManager.UI/Features/Account/Logout/LogoutApiClient.cs`, `RecipeManager.UI/Program.cs` | Sign-in session integration and API-origin decision | AC-06 |
| U-2 | Expose Logout in both signed-in menus and delegate the action to the logout workflow. Do not choose a post-logout route until the product specifies it. | `RecipeManager.UI/Layout/MainLayout.razor`, `RecipeManager.UI/Features/Account/Logout/LogoutView.razor` | U-1; token/session policy | AC-06 |
| U-3 | Handle pending/failure outcomes without raw diagnostics; implement session clearing only after the failure and token policy is agreed. | `RecipeManager.UI/Features/Account/Logout/LogoutView.razor` | U-1, U-2; logout failure policy | AC-06 |

## Test plan
| Level | What is tested | Covers | Task |
|---|---|---|---|
| Manual browser/UI | Logout appears in regular-user and admin layouts. | AC-06 | U-2 |
| Manual network/contract | Selecting Logout sends authenticated `POST /api/auth/logout` with no body and handles success. | AC-06 | U-1 |
| Manual browser/UI | After successful logout, local auth state is cleared after the token policy is resolved; do not assert a redirect until the destination is decided. | AC-06 | U-2 |
| Manual browser/network | Pending and failed request states do not leave controls stuck; failure policy is verified when decided. | AC-06 | U-3 |
| Manual accessibility/responsive | Logout is reachable without hover-only interaction on phone and desktop. | AC-06 | U-2 |

## Risks and open questions
- The logout endpoint deletes refresh tokens but does not revoke already-issued access JWTs; client-side session clearing is required, and an access token may remain valid until its expiry. Confirm the desired user-visible behavior if the endpoint request fails.
- Token persistence and refresh behavior are not implemented in the current UI. Agree the client session policy with sign-in before implementation.
- The PRD does not define a post-logout destination; do not silently assume that logout navigates to `/`.
- No UI localization resources or localization registration currently exist, while the Blazor instructions require a resource-backed Logout label.
