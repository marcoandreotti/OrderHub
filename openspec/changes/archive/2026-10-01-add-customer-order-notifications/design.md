# Design

## Context

See `proposal.md` and the new `communications/customer-order-notifications` spec. The platform already stages transactional outbox messages, processes notification requests, and has SMTP and WhatsApp sender adapters. Public order confirmation and order state transitions are separate application flows; public tracking uses an opaque reference.

## Goals / Non-Goals

**Goals:**
- Stage customer notification work in the same transaction as each relevant order event.
- Reuse notification templates, consent enforcement, SMTP delivery, retries, idempotency, and history.
- Keep order commands independent of SMTP availability.

**Non-Goals:**
- Sending customer order emails over WhatsApp in this change.
- Changing the existing notification administration UI or adding a new provider.
- Promising final inbox delivery; the gateway's accepted status only reflects provider acceptance.

## Decisions

- Represent customer communications as notification requests and outbox messages, not direct SMTP calls from order handlers. This keeps I/O out of the order transaction and reuses the existing retry and delivery history.
- Stage a notification request atomically with the public confirmation or relevant order status transition. Preserve the order event if notification delivery later fails.
- Select event templates by stable purpose and email channel, with per-establishment configuration. Confirmation and later status purposes remain independently configurable; absent destination/template means no request.
- Use an event-derived idempotency key scoped to tenant, establishment, order, purpose, and event occurrence. This prevents duplicate requests while allowing a later distinct status event.
- Populate templates from a minimal order notification read model. Include public order number and opaque public reference/link only; do not expose internal IDs.
- Reuse existing gateway consent behavior. If a selected template requires consent and no valid consent is present, the gateway blocks and records the notification without affecting the order.

## Risks / Trade-offs

- [Risk] Status vocabulary and customer relevance can vary by service type → make customer-visible status purposes configurable and cover supported transition mapping in the spec's implementation tests.
- [Risk] No email may be present for an anonymous checkout → skip notification request and leave the current public tracking flow available.
- [Risk] SMTP acceptance does not prove final delivery → communicate only gateway/provider state in administration and avoid customer-facing delivery guarantees.
- [Risk] A template may omit required placeholders → validate available template parameters before staging, and report a safe configuration failure without rolling back the order.

## Migration Plan

No schema changes are expected if the existing notification and outbox contracts cover the request. Configure an active email template for each desired purpose and use Mailpit in local development. Rollback consists of disabling the automatic staging for these event types; already queued notifications remain governed by existing outbox processing.
