---
status: draft
story: Documentation/ProductBacklog/UI/login-registration/User stories/choose-authentication-form.md
---
# UI plan: Choose an authentication form

## Sources
| Item | Origin | Why needed now |
|---|---|---|
| Landing entry behavior | `Documentation/ProductBacklog/UI/landing-page/User stories/explore-landing-page.md`, AC-02: Sign in or Register opens the shared dialog in the corresponding form mode. | Coordinates the landing-page entry points with the dialog's mode API. |
| Form contents and successful transitions | `Documentation/ProductBacklog/UI/login-registration/User stories/create-account.md`, precondition and AC-07: registration starts in registration mode and success switches to Sign in. | The shared dialog must host the registration workflow and allow its successful mode transition. |
| Sign-in mode entry | `Documentation/ProductBacklog/UI/login-registration/User stories/sign-in.md`, precondition: the shared dialog is open in sign-in mode. | Defines the other mode hosted by the shared dialog. |
| Authentication entry decisions | `Documentation/ProductBacklog/UI/login-registration/prd.md`, R-01: corresponding landing actions open one switchable dialog; `/login` and `/register` are removed and unsupported. | Confirms routes and public-header links must not be used as alternate dialog entry points. |

## Consumed API
| Endpoint (method + route) | Origin (contract / OpenAPI / code path) | DTOs | Used by |
|---|---|---|---|
| None | Dialog open/mode-switch interactions do not themselves make identity API requests. | — | — |

## Backend gaps
- None identified for dialog entry and switching.

## Scope
The shared authentication dialog and its invocation from the landing-page actions; legacy route removal in `Features/Account/Login/LoginPage.razor` and `Features/Account/Register/RegisterPage.razor`; public navigation in `Layout/MainLayout.razor`. Existing UI uses Blazor WebAssembly, MudBlazor, and a route-sensitive `MainLayout`. The prototypes are form-layout references only; their links do not define application routes.

## File structure
```text
RecipeManager.UI/
├── Features/Account/Shared/AuthenticationDialog.razor [create] — shared dialog shell and form-mode switching
├── Features/Account/Login/LoginPage.razor [delete] — remove the separate /login route
├── Features/Account/Register/RegisterPage.razor [delete] — remove the separate /register route
├── Features/Landing/LandingPage.razor [modify] — open the shared dialog from hero actions
├── Layout/MainLayout.razor [modify] — remove public-header auth links and expose Home/Features
└── wwwroot/css/design-system.css [modify] — responsive public navigation/dialog presentation
```

## Tasks
| ID | Description | Files/area | Depends on | Covers (AC-/R-) |
|---|---|---|---|---|
| U-1 | Create one reusable authentication dialog with explicit sign-in and registration modes. It receives workflow components/callbacks and owns only dialog presentation and mode switching. | `RecipeManager.UI/Features/Account/Shared/AuthenticationDialog.razor` | — | AC-01, AC-02, AC-03 |
| U-2 | Wire landing-page Register and Sign in actions to open the same dialog in the selected mode; keep switching within the dialog. | `RecipeManager.UI/Features/Landing/LandingPage.razor`, `RecipeManager.UI/Features/Account/Shared/AuthenticationDialog.razor` | U-1 | AC-01, AC-02, AC-03 |
| U-3 | Remove the routable `/login` and `/register` pages and public authentication links so direct URL visits follow the existing not-found behavior. | `RecipeManager.UI/Features/Account/Login/LoginPage.razor`, `RecipeManager.UI/Features/Account/Register/RegisterPage.razor`, `RecipeManager.UI/Layout/MainLayout.razor` | U-1 | AC-04 |
| U-4 | Apply responsive dialog spacing using the existing design system; retain the existing MudBlazor dialog provider. | `RecipeManager.UI/wwwroot/css/design-system.css` | U-1 | AC-01–AC-03 |

## Test plan
| Level | What is tested | Covers | Task |
|---|---|---|---|
| Manual browser/UI | Register opens the shared dialog in registration mode. | AC-01 | U-2 |
| Manual browser/UI | Sign in opens the same dialog instance/component in sign-in mode. | AC-02 | U-2 |
| Manual browser/UI | Switching modes leaves the dialog open and changes the visible form mode. | AC-03 | U-1, U-2 |
| Manual route/UI | Public header has no Sign in/Register links; `/login` and `/register` do not open the dialog. | AC-04 | U-3 |
| Manual accessibility/responsive | Dialog is operable by keyboard and remains usable on phone and desktop widths. | AC-01–AC-03 | U-4 |

## Risks and open questions
- Registration validation and unexpected request-error presentation, and failed sign-in presentation, are explicitly unresolved in the login-registration PRD. Resolve the common dialog's error presentation with product before treating the plan as ready for approval; see also the create-account and sign-in plans.
- No separate close/cancel interaction requirements are established by this Story; do not add behavioral requirements while implementing.
- No UI localization resources or localization registration currently exist, while the Blazor instructions require resource-backed labels. Decide the resource setup before adding dialog and navigation text.
