---
status: draft
story: Documentation/ProductBacklog/UI/layouts/User stories/navigate-regular-user-features.md
---
# UI plan: Navigate regular-user features

## Sources
| Item | Origin | Why needed now |
|---|---|---|
| Default destination after sign-in | `Documentation/ProductBacklog/UI/login-registration/User stories/sign-in.md`, AC-10: regular users go to Dishes and administrators to Admin Panel overview after successful sign-in. | Confirms the Dishes navigation/default must agree with the sign-in workflow. |
| Regular-user navigation rule | `Documentation/ProductBacklog/UI/layouts/prd.md`, R-02: signed-in menu includes Dishes, Recipes, Ingredients, Cookbooks, Meal Planner, Profile; Dishes is the regular-user default. | Defines the exact required labels and landing destination. |

## Consumed API
| Endpoint (method + route) | Origin (contract / OpenAPI / code path) | DTOs | Used by |
|---|---|---|---|
| None | This Story is navigation-only. Authentication state is supplied by the separate sign-in workflow; no feature data API is required to render the menu. | — | — |

## Backend gaps
- None identified for menu rendering.

## Scope
Signed-in regular-user navigation and default destination. The existing single route-sensitive `RecipeManager.UI/Layout/MainLayout.razor` includes Dishes, Recipes, Cookbooks, Meal plans and Profile, but not Ingredients; `RecipeManager.UI` currently has no regular-user Ingredients page or `/ingredients` route. Existing relevant routes include `/dishes`, `/recipes`, `/cookbooks`, `/meal-plans`, `/profile`; the only Ingredients page found is `/admin/ingredients`.

## File structure
```text
RecipeManager.UI/
├── Layout/MainLayout.razor [modify] — regular-user desktop/mobile menu labels and destinations
├── Shared/Navigation/UiRoutes.cs [create] — shared constants for existing UI routes
└── wwwroot/css/design-system.css [modify] — responsive signed-in navigation
```

## Tasks
| ID | Description | Files/area | Depends on | Covers (AC-/R-) |
|---|---|---|---|---|
| U-1 | Render the required Dishes, Recipes, Ingredients, Cookbooks, Meal Planner and Profile labels in desktop and mobile navigation. Use existing route values for destinations already defined; do not link regular users to the admin-only ingredients page. | `RecipeManager.UI/Layout/MainLayout.razor`, `RecipeManager.UI/wwwroot/css/design-system.css` | Ingredients destination decision for its link | AC-02 |
| U-2 | Define existing route values centrally so the sign-in flow can use Dishes as the regular-user default. Do not add a route for Ingredients until its destination is approved. | `RecipeManager.UI/Shared/Navigation/UiRoutes.cs` | Sign-in Story consumes this route definition | AC-03 |

## Test plan
| Level | What is tested | Covers | Task |
|---|---|---|---|
| Manual browser/UI | Regular-user menu presents all six exact labels in desktop and mobile layouts. Link target for Ingredients remains unasserted until the route is decided. | AC-02 | U-1 |
| Manual browser/UI | Successful regular-user sign-in lands at `/dishes`. | AC-03 | U-2 |
| Manual route/UI | Dishes, Recipes, Cookbooks, Meal Planner and Profile links use existing corresponding routes; Ingredients route behavior is tested only after product confirms its destination. | AC-02 | U-1 |
| Manual accessibility/responsive | All menu entries are reachable and usable at phone and desktop widths. | AC-02 | U-1 |

## Risks and open questions
- **Unresolved route decision:** the Story requires an Ingredients menu item but no destination. No regular-user Ingredients page or `/ingredients` route exists; `/admin/ingredients` is an admin page and must not be silently repurposed. Confirm whether this Story only requires the label, or identify/plan the intended regular-user destination. This is a UI/page-route gap, not a missing backend endpoint.
- Authenticated layouts/session state are not currently present in the UI and depend on the sign-in work.
- No UI localization resources or localization registration currently exist, while the Blazor instructions require resource-backed navigation labels. Resolve the resource setup before implementation.
