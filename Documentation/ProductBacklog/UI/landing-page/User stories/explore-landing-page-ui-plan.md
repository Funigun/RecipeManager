---
status: draft
story: Documentation/ProductBacklog/UI/landing-page/User stories/explore-landing-page.md
---
# UI plan: Explore the Recipe Manager landing page

## Sources
| Item | Origin | Why needed now |
|---|---|---|
| Shared authentication entry and unsupported legacy routes | `Documentation/ProductBacklog/UI/login-registration/User stories/choose-authentication-form.md`, AC-01, AC-02 and AC-04: landing actions open the shared dialog in the selected mode; direct `/login` and `/register` visits do not open it. | Defines the landing-page actions and ensures this Story uses the shared dialog rather than page navigation. |
| Authentication interaction rules | `Documentation/ProductBacklog/UI/login-registration/prd.md`, R-01 and UI/UX needs: one dialog, switchable forms; login and registration entry points are on the landing page. | Provides approved context for connecting the hero actions to the shared dialog. |
| Feature-section content boundaries | `Documentation/ProductBacklog/UI/landing-page/prd.md`, AC-03 and product constraints: retain labels but defer content and component shapes. | Prevents this placeholder slice from expanding into detailed feature designs. |

## Consumed API
| Endpoint (method + route) | Origin (contract / OpenAPI / code path) | DTOs | Used by |
|---|---|---|---|
| None | This Story is presentation/navigation only; authentication requests belong to the dialog's registration and sign-in slices. | — | — |

## Backend gaps
- None identified for the landing-page scope.

## Scope
Landing page at `/`, its public-header navigation in `RecipeManager.UI/Layout/MainLayout.razor`, the existing `Features/Landing/LandingPage.razor`, and the shared authentication-dialog entry point. Keep the existing Blazor WebAssembly hosting model. Current landing page has hero copy and links to `/register` and `/login`; current public header has Home plus those separate authentication links. Implement in the existing UI conventions and make layouts responsive.

## File structure
```text
RecipeManager.UI/
├── Features/Landing/LandingPage.razor [modify] — hero account actions and labeled placeholder feature sections
├── Layout/MainLayout.razor [modify] — public Home/Features navigation without header auth links
└── wwwroot/css/design-system.css [modify] — responsive landing and navigation styling
```

## Tasks
| ID | Description | Files/area | Depends on | Covers (AC-/R-) |
|---|---|---|---|---|
| U-1 | Update the public header to expose Home and Features only; remove public-header Sign in/Register links. Point Features to the landing-page section target. | `RecipeManager.UI/Layout/MainLayout.razor`, `RecipeManager.UI/wwwroot/css/design-system.css` | — | AC-01, AC-04 |
| U-2 | Retain the hero message and put Sign in and Register actions beside it. Wire each action to the shared dialog in the corresponding mode without navigating to `/login` or `/register`. | `RecipeManager.UI/Features/Landing/LandingPage.razor` | Shared dialog/modes from `choose-authentication-form` | AC-01, AC-02 |
| U-3 | Add labeled placeholder sections for Dishes, Recipes, Cookbooks, Meal Plan, and shared units/categories, with a stable target for Features navigation. Keep component details intentionally unspecified. | `RecipeManager.UI/Features/Landing/LandingPage.razor`, `RecipeManager.UI/wwwroot/css/design-system.css` | U-1 | AC-03, AC-04 |

## Test plan
| Level | What is tested | Covers | Task |
|---|---|---|---|
| Manual browser/UI | At `/`, hero and both account actions are present; public header has no authentication links. | AC-01 | U-1, U-2 |
| Manual browser/UI | Register and Sign in open the same dialog, preselected to the corresponding mode, and do not navigate to legacy routes. | AC-02 | U-2 |
| Manual browser/UI | All five named feature groupings render labeled placeholder content. | AC-03 | U-3 |
| Manual browser/UI | Selecting Features moves to the landing-page feature sections. | AC-04 | U-1, U-3 |
| Manual responsive/accessibility | Landing hero, actions, placeholders and navigation remain usable at phone and desktop widths. | AC-01–AC-04 | U-1–U-3 |

## Risks and open questions
- The detailed content and component shapes of feature-section placeholders are intentionally deferred by the target Story.
- The shared dialog is planned as an integration dependency; its implementation and API behavior are separately planned in the authentication Stories.
