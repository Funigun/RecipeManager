# Units domain

## Purpose
Global, admin-maintained measurement units support consistent quantities across users. Within-group conversions are relative to one reference unit; conversions between groups are specific to a global ingredient.

## Entities and value objects
| Name | Kind (entity/value object) | Description | Key attributes |
|------|----------------------------|-------------|----------------|
| Unit | Entity | **Changed:** a global catalog item rather than a user-defined unit. Each unit belongs to a group; exactly one unit in each group is its reference. | Name, short and plural names, unit group, reference designation, factor relative to group reference |
| Global ingredient conversion | Entity | **Added:** a conversion for a specific global ingredient from an alternate unit to that ingredient's primary unit. See [ingredients.md](ingredients.md). | Global ingredient, alternate unit, conversion ratio |

There is no standalone pairwise conversion between units in different groups.

## Relationships
- Each unit belongs to one unit group, which has exactly one reference unit.
- Each non-reference unit's conversion factor is relative to the group's reference unit.
- A cross-group conversion belongs to a global ingredient and maps an alternate unit directly to its primary unit.

## Invariants and rules
| Rule | Applies to | PRD ref |
|------|------------|---------|
| Units are global; users cannot define their own. | Unit | R-06 |
| Each unit group has exactly one reference unit; within-group factors are relative to it. | Unit, unit group | R-26 |
| The unit catalog does not define cross-group conversions; those are specific to a global ingredient and map directly to its primary unit. | Unit, global ingredient conversion | R-07, R-26 |
| Shopping-list amounts normalize to a global ingredient's primary unit when an applicable conversion is available; the group reference unit is not necessarily the normalization target. | Unit, global ingredient conversion | R-20 |

## Lifecycle / states
Not applicable.

## Open questions
None identified in the approved PRD for this domain.
