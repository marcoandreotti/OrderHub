# Tasks

## 1. Validation and API Errors

- [x] 1.1 Translate custom validation messages that bypass FluentValidation's `pt-BR` language manager, including the scheduling-horizon rule; verify invalid requests expose Portuguese field errors.
- [x] 1.2 Verify known API error responses keep Portuguese titles/details and retain validation fields and trace references without exposing internal exception messages.

## 2. Service-Type Presentation

- [x] 2.1 Replace any service-type enum values shown to users in Public Ordering, Operations, and Administration with the existing Portuguese presentation labels; verify the technical values sent to API remain unchanged.
- [x] 2.2 Ensure availability refusals already explained by the public availability notice do not create a duplicate error banner, while unrelated errors still display their support reference; verify both rendering states.

## 3. Verification

- [x] 3.1 Run the relevant API and web validation checks and confirm the localization change introduces no errors.
