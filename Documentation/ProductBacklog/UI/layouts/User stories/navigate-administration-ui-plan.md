---
status: draft
story: Documentation/ProductBacklog/UI/layouts/User stories/navigate-administration.md
---
# UI plan: Navigate administration

## Sources
| Item | Origin | Why needed now |
|---|---|---|
| Regular-user menu inherited by admin | `Documentation/ProductBacklog/UI/layouts/User stories/navigate-regular-user-features.md`, AC-02: regular-user menu has Dishes, Recipes, Ingredients, Cookbooks, Meal Planner and Profile. | The admin menu must present all regular-user items before adding Admin Panel. |
| Administrator sign-in default | `Documentation/ProductBacklog/UI/login-registration/User stories/sign-in.md`, AC-10: administrator is taken to Admin Panel overview. | Connects role-specific successful sign-in navigation to the overview route. |
| Admin navigation and sidebar rule | `Documentation/ProductBacklog/UI/layouts/prd.md`, R-03: admin menu includes all regular items plus Admin Panel; Admin Panel opens overview and is the admin default; existing sidebar remains. | Defines menu composition, default and retained sidebar behavior. |

## Consumed API
| Endpoint (method + route) | Origin (contract / OpenAPI / code path) | DTOs | Used by |
|---|---|---|---|
| None | This Story is navigation-only; administrator role comes from the sign-in session established by the separate authentication workflow. | — | — |

## Backend gaps
- None identified for the admin navigation or overview.

## Scope
Administrator navigation in `RecipeManager.UI/Layout/MainLayout.razor`, existing `/admin` overview and the current admin sidebar. The current top menu is a route-sensitive shared menu without an Admin Panel item; the existing admin sidebar includes overview and administration catalog links. `/admin` already exists. The regular-user Ingredients route remains unresolved as recorded in the regular-user plan.

## File structure
```text
RecipeManager.UI/
├── Layout/MainLayout.razor [modify] — administrator top-menu entry and role-aware layout selection
├── Shared/Navigation/UiRoutes.cs [modify] — shared route value for the existing /admin overview
└── wwwroot/css/design-system.css [modify] — responsive admin top-menu styling
```

## Tasks
| ID | Description | Files/area | Depends on | Covers (AC-/R-) |
|---|---|---|---|---|
| U-1 | Reuse the regular-user menu entries and add Admin Panel targeting the existing `/admin` overview. Determine administrator layout selection from stable role state, not display text. | `RecipeManager.UI/Layout/MainLayout.razor`, `RecipeManager.UI/Shared/Navigation/UiRoutes.cs`, `RecipeManager.UI/wwwroot/css/design-system.css` | Regular-user routes; sign-in role-state decision | AC-04 |
| U-2 | Keep the existing admin sidebar and its catalog links available beneath the `/admin` overview. The post-sign-in destination is implemented by the dependent sign-in slice, not duplicated here. | `RecipeManager.UI/Layout/MainLayout.razor` | U-1 | AC-05 |
| U-3 | Verify the admin menu, overview default after sign-in, retained sidebar and responsive navigation with the sign-in slice integrated. | `RecipeManager.UI/Layout/MainLayout.razor` | U-1, U-2; sign-in Story's route-selection task | AC-04, AC-05 |

## Test plan
| Level | What is tested | Covers | Task |
|---|---|---|---|
| Manual browser/UI | Admin top menu contains all regular-user items plus Admin Panel. | AC-04 | U-1 |
| Manual browser/UI | Successful administrator sign-in navigates to `/admin` overview. | AC-05 | U-3 |
| Manual browser/UI | Admin overview retains the existing sidebar and its administration-page links. | AC-05 | U-2 |
| Manual accessibility/responsive | Admin top-menu access and sidebar navigation remain usable at phone and desktop widths. | AC-04, AC-05 | U-3 |

## Risks and open questions
- The regular-user plan identifies an unresolved Ingredients destination; the admin menu's inherited Ingredients label must follow the same product resolution, not guess a new route.
- Current UI has no authentication-state/role-aware layout infrastructure. The sign-in response contains role claims, but integration and authorization expectations should be agreed as part of the auth work.
- No UI localization resources or localization registration currently exist, while the Blazor instructions require resource-backed navigation labels. Resolve the resource setup before implementation.
