## Context

See `proposal.md` for motivation and the four delta specs for observable behavior.

The frontend already uses Vue 3, Quasar, TypeScript and Pinia with four official shells: `PublicLayout`, `AdministrationLayout`, `OperationsLayout` and `PlatformLayout`. The public ordering, administrative CRUD, order cockpit and KDS are functional and tested, so the remodel must preserve contracts and evolve by vertical slice rather than replace the application.

The current visual foundation is intentionally small: `_tokens.scss` defines six colors, one radius and one font family; `ProblemBanner` is the only documented global component. Administration repeats tables, dialogs, missing-unit banners and page-heading structures, but their responsibilities still differ. Operations and KDS contain local card, timing and status styles appropriate to their separate purposes.

Public theme values are currently applied on `document.documentElement`. Because the same global variables are consumed by other shells, a public Tenant theme can persist beyond its intended surface. The redesign must remove this cross-surface coupling before expanding theme coverage.

## Goals / Non-Goals

**Goals:**

- Establish scoped visual foundations without changing the application stack.
- Preserve distinct Public, Administration, Operations, KDS and Platform identities.
- Make new components discoverable and classify them before promotion to shared scope.
- Deliver each surface incrementally with behavioral, responsive and accessibility tests.
- Keep existing command contracts, authorization, Tenant isolation and server-authoritative pricing unchanged; the operational read summary may receive the additive item preview required by its card.
- Coordinate the future administrative dashboard with `add-reporting-dashboard` instead of creating duplicate projections.

**Non-Goals:**

- Rebuild the frontend or replace Quasar, Pinia or Vue Router.
- Introduce Storybook, another component framework, a CSS-in-JS runtime or a new state library.
- Redesign backend contracts, domain behavior, CQRS, persistence or authentication.
- Create wrappers for every Quasar component.
- Force every existing local CSS value into a token during the first phase.
- Unify Operations and KDS cards into one generic component.

## Decisions

### 1. Scope theme variables by surface

Each official layout will expose a stable surface root. Semantic tokens will resolve beneath that root rather than being mutated globally for every experience. Public Tenant branding will be applied to the `PublicLayout` subtree and reset naturally when that layout unmounts. Administration, Operations, KDS and Platform will use owned semantic mappings.

The existing `--oh-*` tokens can remain as a compatibility layer during migration. New tokens will be introduced only for repeated semantic roles such as surface, border, secondary text, focus, success, warning, danger, operational urgency and density. Primitive values and semantic aliases will remain separate when that distinction has real consumers.

**Alternatives considered:** continuing to mutate `documentElement` is simpler but allows theme leakage; duplicating a complete token set per page avoids leakage but produces drift. Layout-scoped semantic aliases provide isolation with an incremental migration path.

### 2. Keep the four shells and specialize within them

`PublicLayout`, `AdministrationLayout`, `OperationsLayout` and `PlatformLayout` remain the official routing shells. KDS remains routed through Operations but receives a dedicated KDS page mode with reduced chrome and production-specific density. A single `MainLayout` will not be introduced.

Public will own Tenant branding, mobile navigation and cart access. Administration will own stable navigation, unit context and predictable page headers. Operations will own connection state, immediate work and actions. KDS will own distance readability and production controls.

**Alternatives considered:** one highly configurable layout reduces file count but makes permissions, density, navigation and responsive behavior conditional and harder to test.

### 3. Build components from proven semantics, not Quasar tags

Quasar remains the low-level component toolkit. A shared component is created only when it adds stable application semantics, accessibility, behavior or visual policy.

Current component map:

| Action | Items | Decision |
| --- | --- | --- |
| Reuse | `ProblemBanner`; four official layouts; current session, router and store boundaries | Preserve APIs unless a delta requirement demands extension |
| Extend | Semantic tokens; public theme application; consistent administrative table, dialog and missing-unit treatments | Extend policies first; extract components only after at least two real consumers converge |
| Create in feature | Public product card, category navigation, persistent cart bar and product composer; administration catalog view switch and product summary; operations order card; KDS ticket | Keep behavior close to its domain and test independently |
| Keep local | `CatalogPicker`, `AccessStep`, `UserPermissions`, module editors, confirmation content and unique page composition | Do not promote without a second semantically equivalent consumer |

`AppButton`, `AppCard`, `AppDialog`, `AppStatus` and `AppPageHeader` are not assumed to exist. During migration, repeated patterns must first be normalized in two consumers. Promotion to global scope requires a documented purpose, consumer list, API, examples and behavior tests in `docs/web-design-system.md`.

**Alternatives considered:** creating the full proposed `App*` library up front would make screens superficially uniform but introduces speculative APIs and generic wrappers prohibited by repository rules.

### 4. Treat wireframes as behavioral layout contracts

Wireframes define hierarchy and responsive composition, not final illustration or pixel values.

- Public mobile: establishment context, availability, search, category navigation, product feed and persistent cart action; product composition uses a full-height mobile surface with a persistent quantity/add action.
- Public desktop: the same information hierarchy may use a wider product grid and adjacent cart summary without creating a different journey.
- Administration: persistent navigation and unit context, predictable page header, filters, primary action and content area; catalog switches between visual and compact renderers over the same query state.
- Operations: connection and synchronization remain adjacent to the live board; order cards expose time, service type, essential items and next action without requiring detail expansion.
- KDS: reduced navigation, large production tickets, redundant urgency signals and large actions; metadata that does not affect preparation is visually subordinate.

Responsive changes will use CSS and Quasar layout facilities first. JavaScript media branching is reserved for behavior that cannot be expressed through responsive layout.

### 5. Preserve state and data ownership while changing renderers

Visual and compact catalog views will consume the same filters, page, result set and editor actions. Switching view mode will not issue a semantically different query or discard state. The public composer continues to submit the exact fraction and option data already modeled; it does not calculate authoritative prices. Operations and KDS continue to use their existing stores, realtime channel and polling fallback.

The operations search read model will project a bounded item preview with product name, variation and quantity in the same Dapper query used by the queue. The preview is additive to `OrderSummaryResponse`, tenant-scoped and contains no domain entity or frontend-derived financial value. It avoids loading details per card and prevents an N+1 request pattern. Full modifiers, notes and history remain owned by the detail contract and by KDS where production requires them.

**Alternatives considered:** separate state per renderer simplifies individual components but risks divergent filters, pagination and mutations. Fetching detail once per operational card was rejected because it creates request amplification, inconsistent snapshots and unnecessary load.

### 6. Migrate by vertical slice with a compatibility layer

The implementation order is foundation, Public, Operations/KDS, Administration shell/catalog and remaining administrative modules. Each slice updates its tests and browser checks before the next begins. Existing global styles stay in place until their final consumer moves; removal happens only after repository search confirms no remaining use.

The reporting dashboard is integrated after its read models and definitions are available from `add-reporting-dashboard`. This change may style and place those widgets but will not invent revenue, comparison or product metrics locally.

### 7. Validate behavior and visuals at supported breakpoints

Vitest continues to cover state, permissions and interaction. Browser-level checks will cover at least the supported mobile Public viewport, tablet Administration/Operations viewport and desktop or monitor layouts. Verification includes overflow, keyboard order, focus visibility, touch targets, contrast, non-color status cues and preservation of loading, empty, error and disabled states.

Snapshots alone are insufficient. Assertions must verify accessible names, state changes and preserved data. Visual screenshots are review evidence, not the only acceptance mechanism.

## Risks / Trade-offs

- **[Risk] Public theme continues leaking through root variables during partial migration** → Introduce the scoped surface root first and retain compatibility aliases only inside explicit surfaces.
- **[Risk] A broad design-system phase delays visible improvements** → Add only tokens and shared components required by the next vertical slice.
- **[Risk] Generic wrappers hide useful Quasar APIs and become difficult to evolve** → Require proven consumers and semantic responsibility before promotion.
- **[Risk] Cards and larger controls reduce information density in Administration** → Provide compact and visual catalog renderers over shared state; apply density by surface rather than globally.
- **[Risk] Operations and KDS become visually similar despite different jobs** → Keep feature-specific card and ticket components with separate hierarchy and validation scenarios.
- **[Risk] Accessibility regresses during styling work** → Pair every migrated slice with keyboard, focus, contrast, touch and non-color assertions.
- **[Risk] Work overlaps `add-reporting-dashboard`** → Treat its projections and financial definitions as authoritative and integrate only after its contracts are available.
- **[Trade-off] Incremental migration temporarily retains legacy and new tokens** → Track remaining consumers and remove compatibility rules in a dedicated cleanup task after all slices migrate.

## Migration Plan

1. Add surface scoping, semantic token foundations and documentation without changing page behavior.
2. Remodel Public navigation, cards, persistent cart and composer; verify mobile-first behavior and server-authoritative totals.
3. Remodel Operations and KDS using separate feature components; verify realtime fallback, touch and distance-readable states.
4. Normalize the Administration shell and page hierarchy, then implement catalog visual/compact renderers over shared state.
5. Migrate remaining administrative pages only to proven patterns; promote shared components when consumer evidence exists.
6. Integrate reporting widgets when `add-reporting-dashboard` is ready.
7. Remove unused compatibility styles and update the component catalog.

Each phase is independently releasable. Rollback reverts the migrated slice while retaining existing API and store behavior; no database rollback is required.
