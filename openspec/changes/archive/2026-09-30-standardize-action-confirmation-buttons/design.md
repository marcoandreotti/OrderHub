## Context

See `proposal.md` for motivation and `specs/web/action-controls/spec.md` for the behavior contract. The Vue application already uses Quasar `QBtn`, has an approved semantic palette, and exposes distinct Public, Administration, Operations/KDS, and Platform shells.

## Goals / Non-Goals

**Goals:**
- Make repeated item actions compact and recognizable without relying on hover.
- Make data-changing confirmation actions explicit and visually primary.
- Preserve established surface identity, semantic colors, labels, loading states, and action behavior.

**Non-Goals:**
- Change permissions, business rules, API calls, action order, or confirmation requirements.
- Restyle navigation, filters, dialog close/back/cancel controls, or standalone recovery controls as collection actions.
- Add a new button library or a generic component that only forwards Quasar props.

## Decisions

- Use Quasar `QBtn` directly. Its `square`, `icon`, `icon-right`, `label`, `color`, and loading/disabled props already express this pattern without a new abstraction.
- For repeated item actions, use an action-specific icon, Quasar square shape, the existing semantic color, `aria-label` and a `QTooltip` when no visible label is present. Preserve a visible label when it is needed for operational recognition, including order and kitchen transitions. Keep an accessible target of at least 44px and never make hover necessary to discover the action.
- For confirmation or form-submit actions, use an action-specific leading icon and visible localized label. Keep cancel/back actions secondary and text-led. Preserve destructive coloring and operation-specific wording.
- Do not use `glossy` as a global style. Keep OrderHub's orange/graphite palette and semantic success/warning/danger colors.
- Record the global rules in `docs/web-design-system.md`; do not create `AppButton` because no behavior beyond Quasar's native props requires a wrapper.

## Risks / Trade-offs

- Icon-only actions can be ambiguous → pair each with an accessible name and tooltip, and choose familiar icons.
- More filled square controls can add visual weight to dense rows → reserve the color for the action's meaning and keep labels in the tooltip/accessibility tree.
- Icon glyph availability can differ → use names from the configured Material SVG icon set and verify through the frontend build.

## Migration Plan

Update collection action consumers and confirmation CTAs incrementally within the same frontend release. Rollback consists of restoring the previous `QBtn` props; no persisted data or API contract changes.
