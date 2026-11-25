---
status: approved
topic: Domain redesign — Units
source: decisions.md
---
# Units

## Summary
Make units global and define within-group conversions relative to one reference unit. Cross-group conversions belong to global ingredients rather than the unit catalog.

## Users and permissions
See the shared roles and permissions in the [domain redesign overview](../overview.md#users-and-permissions).

## Domain rules

### Units and conversions

- **R-06 — Global units** *(Entities: Unit; Source: D-06)*
  - Units and unit conversions are global; users cannot define their own units.
- **R-26 — Unit groups and conversion scope** *(Entities: Unit, unit group, global ingredient conversion; Source: D-26)*
  - Each unit group has one reference unit.
  - Within-group conversions are expressed relative to that reference unit; conversion between different groups is ingredient-specific.

## User stories

### US-08 — Use global units
As a user, I want to use the global unit catalog so that I can use consistent units without defining my own.

**Acceptance criteria**
- **AC-06:** Given a user is managing their own data, when they need a unit or unit conversion, then they can use the global catalog but cannot create a user-specific unit or conversion.

**Related domain rules:** R-06

### US-09 — Maintain global unit groups
As an administrator, I want to maintain global unit groups and their reference units so that within-group conversions are consistent.

**Acceptance criteria**
- **AC-24:** Given units belong to the same group, when their conversions are defined, then exactly one unit in the group is its reference and every other unit's conversion is expressed relative to it; the unit catalog does not define conversions between different groups.

**Related domain rules:** R-06, R-26

## Impact on existing system
The current `Unit` model has a unit group, base-unit flag, primary-unit reference, and conversion factor. Units become global. Each group has one reference unit, and within-group conversion factors are relative to it. Cross-group conversions are specific to a global ingredient and map an alternate unit to its primary unit; see [Ingredients](../ingredients/prd.md#domain-rules).

## Documentation targets
- Update the Units explanation in `wiki/domain`.
