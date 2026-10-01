# Tasks

## 1. Order event contracts and persistence

- [ ] 1.1 Identify all public confirmation and customer-visible status transition write paths; document the stable event identity and verify each path can stage outbox work within its transaction.
- [ ] 1.2 Add notification request creation from order events using existing template, consent, tenant scope, and outbox contracts; verify Application and Infrastructure unit tests cover missing destination/template, consent blocking, and duplicate event processing.
- [ ] 1.3 Add or update persistence integration tests proving the order transition and outbox notification request commit or roll back atomically.

## 2. Template data and customer-safe content

- [ ] 2.1 Provide the minimum order notification data needed by templates, including public order number and opaque tracking reference; verify tests assert internal order and tenant IDs are absent.
- [ ] 2.2 Configure supported event purposes and template placeholders in the existing administration flow; verify template validation and the Portuguese customer email content.

## 3. Delivery and end-to-end evidence

- [ ] 3.1 Verify transient SMTP failure schedules retry and terminal/uncertain outcomes remain visible in notification history without changing order state.
- [ ] 3.2 Document local SMTP/Mailpit setup and the manual order-confirmation/status-change procedure; verify the instructions produce an email visible in Mailpit.
- [ ] 3.3 Run focused backend tests and an end-to-end local flow for confirmation plus at least one later customer-visible status, verifying idempotency and the tracking link.
