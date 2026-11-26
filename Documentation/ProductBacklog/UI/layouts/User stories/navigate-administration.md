---
status: ready
depends_on:
  - Documentation/ProductBacklog/UI/layouts/User stories/navigate-regular-user-features.md
---
# Navigate administration

As an admin user, I want access to both regular-user areas and administration so that I can manage the application from the admin layout.

## Behavior

- The admin top menu presents all regular-user menu items plus Admin Panel.
- Admin Panel opens the admin overview and is the default view after an administrator logs in.
- The existing admin sidebar remains available for navigating administration pages.

## Acceptance criteria

- **AC-04:** The admin top menu presents all regular-user menu items plus Admin Panel.
- **AC-05:** After an administrator logs in, the default view is the admin overview; the existing admin sidebar remains available for administration-page navigation.

## Traceability

- Acceptance criteria: AC-04, AC-05
- Domain rules: R-02, R-03
