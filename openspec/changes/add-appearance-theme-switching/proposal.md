## Why

OrderHub has a single light palette today, so operators and customers cannot choose the viewing mode that suits their device or environment. A consistent preference across the authenticated and public surfaces will make the product easier to use while keeping each surface and each establishment's brand distinct.

## What Changes

- Add a shared appearance control with System, Light, and Dark choices to Public, Administration, Operations/KDS, and Platform navigation.
- Use System as the initial preference, persist explicit choices in the current browser, and apply the selected palette consistently without a full-page reload.
- Extend semantic design tokens and Quasar dark-mode behavior for neutral surfaces, text, controls, overlays, and borders.
- Keep tenant brand colors and identity isolated to Public; adapt public neutral surfaces and text for the selected appearance with accessible contrast.
- Document the preference behavior and shared control in the web Design System.

## Capabilities

### New Capabilities
- `web/appearance-preferences`: A cross-surface appearance preference with system, light, and dark modes.

### Modified Capabilities
- None.

## Impact

- Affected frontend code: global theme tokens and bootstrapping, Quasar dark mode, shared components, Public/Administration/Operations/Platform layouts, tenant-theme application, and public surface styling.
- No API, database, or backend dependency. The preference is browser-local for this change.
- No change to tenant authorization, order behavior, or public brand configuration.
