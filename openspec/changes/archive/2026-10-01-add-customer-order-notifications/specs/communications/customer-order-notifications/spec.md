# Spec Delta

## Purpose

Connect order lifecycle events to customer-facing transactional messages so customers can follow important order progress without repeatedly opening the public tracking page.

## ADDED Requirements

### Requirement: Customer receives an email after public order confirmation
When a confirmed public order has a customer email and a matching active template, the system SHALL request one transactional email through the notification gateway.

#### Scenario: Confirmation has an email destination and active template
- **WHEN** a public order is confirmed and a matching email template is active
- **THEN** the system records a notification request for the customer's email in the same transaction as the order confirmation
- **AND** the message includes the order number and opaque public tracking reference

#### Scenario: Confirmation has no email destination or matching template
- **WHEN** a public order is confirmed without a customer email or without a matching active template
- **THEN** the order confirmation succeeds
- **AND** no customer email is requested

### Requirement: Relevant order status transitions request customer email
The system SHALL request a customer email for configured, customer-visible status transitions of a public order when the order has an email destination and a matching active template.

#### Scenario: Order enters a configured customer-visible status
- **WHEN** an order transitions to a status configured for customer notification
- **THEN** one notification request is recorded with the transition and public tracking reference
- **AND** the notification request is committed atomically with the status transition

#### Scenario: Order transition is not configured for customer notification
- **WHEN** an order transitions to a status without a configured customer template
- **THEN** the status transition succeeds without creating a notification request

### Requirement: Customer order notifications are idempotent and do not block order processing
The system SHALL prevent duplicate customer notifications for the same order event and SHALL keep notification delivery failures independent from order confirmation and status transitions.

#### Scenario: Order event is processed more than once
- **WHEN** the same confirmation or status transition is handled repeatedly
- **THEN** no more than one notification request is created for that order event, channel, and purpose

#### Scenario: Email delivery fails after the order event commits
- **WHEN** the SMTP provider rejects or cannot accept the notification
- **THEN** the order confirmation or status transition remains committed
- **AND** the notification gateway records the failed or retryable delivery outcome

### Requirement: Tracking links do not expose internal order identifiers
Customer order emails SHALL identify an order only with its public order number and opaque tracking reference.

#### Scenario: Email is rendered for a customer order
- **WHEN** the notification template is populated with order data
- **THEN** the email contains the public tracking reference or URL
- **AND** it does not contain the internal order ID or tenant ID
