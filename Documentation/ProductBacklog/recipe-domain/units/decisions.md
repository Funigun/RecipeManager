---
status: approved
topic: Domain redesign — Units
---
# Unit Decisions

## Decisions

### Units and conversions

- <u>D-06 — Global units (User)</u>

  **Decision:** Units and unit conversions are global; users cannot define their own units.

  **Reason:** The target state explicitly removes user-defined unit catalogs.

- <u>D-26 — Unit conversion scope and model (User)</u>

  **Decision:** Each unit group has one reference unit, and within-group conversions are defined relative to it. Cross-group conversions are defined per global ingredient and map an alternate unit to that ingredient's primary unit.

  **Reason:** One reference unit keeps within-group conversions simple for admins; ingredient-specific cross-group mappings avoid assuming a universal relationship such as cup-to-gram.
