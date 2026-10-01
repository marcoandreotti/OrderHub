## Context

See `proposal.md` for motivation and `specs/web/appearance-preferences/spec.md` for observable behavior. The web app has separate Quasar layouts for Public, Administration, Operations/KDS, and Platform. Current semantic tokens default to light values, while public tenant theme values are written directly to the public surface.

## Goals / Non-Goals

**Goals:**
- Keep one appearance preference and one control shared by all surface layouts.
- Resolve System from `prefers-color-scheme`, persist the selected mode in browser storage, and apply the resolved mode before Vue renders.
- Let Quasar components and overlays follow the same resolved mode as the semantic CSS tokens.
- Separate tenant identity colors from public neutral surface tokens so dark appearance can adapt neutrals without changing tenant brand colors.

**Non-Goals:**
- Sync the preference between browsers or devices, or add a backend profile setting.
- Change tenant authorization, order state colors, or public theme configuration.
- Force dark colors onto images or tenant brand assets.

## Decisions

- **Use a small global appearance module, not session state.** Store `system | light | dark` in a reactive frontend module, initialize from `localStorage`, and listen to both `matchMedia` and the browser `storage` event. Appearance is device-local presentation state and does not belong in authentication/session data.
- **Apply the resolved mode through the document theme attribute and Quasar Dark plugin.** Set `data-theme="light|dark"` on the root for semantic CSS tokens, and use Quasar Dark so menus, dialogs, selects, and other framework surfaces follow the same mode. Add a synchronous pre-render initializer to avoid a light-theme flash.
- **Define paired semantic token sets.** Keep light defaults and add dark values for page/surface, text, border, navigation, and focus. Keep brand and status tokens semantic and ensure text variants remain readable on both neutral sets.
- **Keep tenant colors in public-specific properties.** `applyPublicTheme` stores tenant background and text as public identity inputs rather than overriding global neutral tokens. Light public appearance may use those configured neutrals; dark appearance uses the dark neutral set and accessible text while preserving the tenant's primary/secondary identity.
- **Use one shared `AppearanceControl` in each shell.** Place it in Public, Administration, Operations/KDS, and Platform headers. The compact trigger opens a labeled menu with System, Claro, and Escuro, including the current selection. Keep its text and focus behavior accessible and avoid dependence on a missing icon font.

## Risks / Trade-offs

- **[Existing fixed light utility classes can remain bright in dark mode]** → Search frontend sources for fixed neutral colors and adapt only shared or user-visible neutral cases; keep semantic status colors and tenant imagery intact.
- **[Tenant-specified text/background combinations may not work on dark surfaces]** → Validate contrast for public brand actions and use a readable dark-mode foreground on dark neutrals, while retaining tenant primary colors when a contrasting foreground exists.
- **[A local preference is not portable across devices]** → Keep browser-local storage explicit in the UI/spec and leave account synchronization for a separately scoped capability.

## Migration Plan

No data migration is required. Browsers without a saved choice start in System mode. Rollback removes the shared control and theme initializer and restores light token defaults; stored browser preference can safely be ignored.
