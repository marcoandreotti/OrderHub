# Design

## Context

See `proposal.md` and `specs/web/portuguese-user-interface/spec.md`. The API already sets FluentValidation's default language to `pt-BR` in `Program.cs`, and `GlobalExceptionMiddleware` emits localized ProblemDetails titles and generic details. Validation failures can still carry explicit custom messages, and service-type labels are rendered by feature-local frontend mappings. The public ordering page also suppresses a duplicate availability error when the same notice is already visible.

The API contracts use stable English enum values such as `Table`, `Pickup`, and `Delivery`; those values are consumed by persistence and clients and must remain unchanged.

## Goals / Non-Goals

**Goals:**

- Complete Portuguese localization at existing API and UI presentation boundaries.
- Preserve generic user-facing error details and support references without exposing internal exception messages.
- Keep technical enum values stable while presenting Portuguese labels.
- Avoid showing the same public availability refusal twice.

**Non-Goals:**

- Add language selection, resource files, or a general localization framework.
- Translate user-authored content, internal logs, identifiers, routes, or API enum values.
- Redesign error or availability components.

## Decisions

- Keep FluentValidation's `pt-BR` language manager configuration in the API composition root. Translate explicit custom validator messages that bypass its built-in translations; do not change domain rules or exception messages solely for display because the global middleware maps known exceptions to safe localized ProblemDetails.
- Keep ProblemDetails mapping centralized in `GlobalExceptionMiddleware`. Preserve localized titles, generic details, validation field errors, trace IDs, and typed extensions. Do not expose `Exception.Message`; unexpected server errors continue to omit detail.
- Localize service-type labels at existing feature presentation mappings in Public Ordering, Operations, and Administration. Continue sending and receiving the existing enum strings unchanged; do not introduce a shared localization abstraction for these few module-local labels.
- Suppress a duplicate Public Ordering error by matching the API's typed availability `reason` to the selected service's displayed availability reason. Do not compare display strings, because a custom availability message can differ from the API's generic localized detail. Unrelated errors remain in `ProblemBanner` with their support reference.

Alternatives considered: translating exceptions in Domain/Application would couple user-facing language to business rules; changing serialized enum names would break API contracts; adding a global i18n package is broader than the current single-locale requirement.

## Risks / Trade-offs

- Explicit `.WithMessage(...)` strings can bypass FluentValidation's language manager → audit custom messages in API validators and translate only user-visible validation text.
- A frontend label map can omit a new enum value → keep maps exhaustive against their local TypeScript union and add focused coverage.
- Availability details may diverge between the API and the page notice → compare stable reason codes and test both suppression and normal error rendering.

## Migration Plan

No data or contract migration is required. Deploy the API and web changes together. Rollback consists of reverting the localized message and presentation changes; stored enum values and persisted data remain compatible.
