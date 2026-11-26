---
status: draft
story: Documentation/ProductBacklog/UI/layouts/User stories/navigate-public-pages.md
---
# UI plan: Navigate public pages

## Sources
| Item | Origin | Why needed now |
|---|---|---|
| Shared authentication entry | `Documentation/ProductBacklog/UI/login-registration/User stories/choose-authentication-form.md`, AC-01, AC-02 and AC-04: landing actions select a mode in the shared dialog; public header has no authentication links and legacy URLs do not open it. | The public navigation Story explicitly relies on the shared dialog behavior. |
| Landing action and feature anchors | `Documentation/ProductBacklog/UI/landing-page/User stories/explore-landing-page.md`, AC-02 and AC-04: landing actions open the dialog and Features navigation takes visitors to feature sections. | Defines where public account entry and Features navigation lead. |
| Public navigation rule | `Documentation/ProductBacklog/UI/layouts/prd.md`, R-04: base layout provides Home and Features; authentication actions are on the landing page and absent from the public header. | Supplies the approved public navigation model. |

## Consumed API
| Endpoint (method + route) | Origin (contract / OpenAPI / code path) | DTOs | Used by |
|---|---|---|---|
| None | This Story concerns public links and landing-page dialog entry; the dialog's form workflows consume identity APIs in separate Stories. | — | — |

## Backend gaps
- None identified.

## Scope
Public branch of `RecipeManager.UI/Layout/MainLayout.razor`, Home/Features navigation, and the landing-page links to the shared authentication dialog. Existing public navigation currently includes Home, Sign in and Register; landing action links currently navigate to separate routes. Preserve Blazor WebAssembly and existing responsive desktop/mobile navigation patterns.

## File structure
```text
RecipeManager.UI/
├── Features/Landing/LandingPage.razor [modify] — connect hero actions to the shared dialog
├── Layout/MainLayout.razor [modify] — Home/Features public navigation and no header auth links
└── wwwroot/css/design-system.css [modify] — responsive desktop/mobile navigation styling
```

## Tasks
| ID | Description | Files/area | Depends on | Covers (AC-/R-) |
|---|---|---|---|---|
| U-1 | Expose Home and Features in desktop and mobile public navigation; remove header authentication links. Features targets the named landing-page section. | `RecipeManager.UI/Layout/MainLayout.razor`, `RecipeManager.UI/wwwroot/css/design-system.css` | — | AC-01 |
| U-2 | Connect landing-page Sign in and Register actions to the shared dialog in their corresponding modes; do not navigate to separate auth pages. | `RecipeManager.UI/Features/Landing/LandingPage.razor` | U-1; dialog behavior from `choose-authentication-form` | AC-01 |
| U-3 | Verify responsive navigation and active/focus states without adding separate public routes. | `RecipeManager.UI/Layout/MainLayout.razor`, `RecipeManager.UI/wwwroot/css/design-system.css` | U-1, U-2 | AC-01 |

## Test plan
| Level | What is tested | Covers | Task |
|---|---|---|---|
| Manual browser/UI | Base/public layout shows Home and Features and no Sign in/Register header links. | AC-01 | U-1 |
| Manual browser/UI | Landing Sign in/Register actions open the shared dialog in the matching mode, without changing to `/login` or `/register`. | AC-01 | U-2 |
| Manual browser/UI | Features navigation moves to feature sections; mobile navigation exposes the same public destinations. | AC-01 | U-1, U-3 |
| Manual accessibility/responsive | Public navigation works on phone/desktop and is keyboard-operable. | AC-01 | U-3 |

## Risks and open questions
- The target specifies public Home and Features but not additional public destinations; do not add About or authentication links.
- Dialog error behavior is specified in the registration/sign-in Stories and remains an open product decision there.
- No UI localization resources or localization registration currently exist, while the Blazor instructions require resource-backed application text. Decide the resource setup before adding localized navigation labels.
