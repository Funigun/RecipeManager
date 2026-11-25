# MVP UI Specification

This document describes the visual and interaction guidance for the MVP. Product capabilities and domain behavior are defined in [Specification.md](Specification.md). Detailed design tokens remain in the [UI mockup design reference](../../RecipeManager.UI/Mockups/design.md).

## Design principles

- Keep recipe and ingredient information organized, readable, and quick to scan while cooking.
- Use a bright, warm kitchen aesthetic with botanical colors, natural accents, food photography, and restrained shadows.
- Prioritize touch-friendly controls and legibility at arm's length.

## Responsive layout

- **Mobile (< 768px):** Use a four-column layout, 1rem margins and gutters, edge-to-edge ingredient rows, and bottom-positioned primary actions.
- **Tablet (768px–1023px):** Use an eight-column layout and support split views with ingredients alongside scrollable preparation steps.
- **Desktop (≥ 1024px):** Use a twelve-column layout, 3rem margins, and a content width capped at 1320px.

## Visual system

- Use the colors and surface tokens in the design reference for light and dark surfaces, text, actions, and status indicators.
- Use Outfit for display and heading typography and Plus Jakarta Sans for body text and numerical values.
- Use clear elevation and rounded surfaces to distinguish cards, active steps, dialogs, and cooking controls without harsh shadows.
- Use botanical green for primary actions and positive states; use warm spice accents for active cooking and timer actions.

## Component guidance

- **Recipe cards:** Use prominent food photography, cook-time and difficulty indicators, and dietary badges where applicable.
- **Ingredient lists:** Use large touch targets (at least 44px high), interactive checkboxes, and clear measurement controls.
- **Recipe steps:** Use numbered steps with a distinct active state and readable instructions.
- **Cookbooks:** Present collections as customizable binders or covers with a title, recipe count, and recipe previews.
- **Cook mode:** Provide a high-contrast, full-screen cooking view with enlarged instructions, ingredient callouts, and gesture-friendly previous/next controls.
- **Timers:** Render recognized time references as interactive controls that can start a countdown.
- **Actions:** Distinguish primary, secondary, and active timer/cook-mode actions through color and shape.

## Interaction details described by the design reference

- Ingredient checklists mark completed items with a check and strikethrough.
- A segmented control switches ingredient measurements between Metric and Imperial.
- Inline timer controls open a dockable countdown with audio-visual cues.
- Cook mode enables screen wake-lock by default.

These interactions are design-reference proposals; confirm their MVP product scope against [Specification.md](Specification.md) before implementation.

## Open UI decisions

- Resolve differences between the design reference's token values and its prose descriptions before treating either as canonical.
- Define screen-level flows, loading, empty, and error states.
- Specify accessibility requirements, including keyboard and assistive-technology behavior.
