# Spec Delta

## ADDED Requirements

### Requirement: Docker Compose applies pending database migrations before starting the API
The local Docker Compose environment SHALL apply all pending PostgreSQL migrations before the API begins serving requests, and SHALL report a migration failure as a failed startup dependency.

#### Scenario: Database is new or has pending migrations
- **WHEN** a developer starts the Docker Compose environment with a healthy PostgreSQL service
- **THEN** the migrations process completes successfully before the API starts
- **AND** the API starts against the migrated schema

#### Scenario: Database schema is already current
- **WHEN** a developer starts the environment and no migrations are pending
- **THEN** the migration step completes successfully without changing the schema
- **AND** the API starts normally

#### Scenario: Migration fails
- **WHEN** the migration process cannot apply a pending migration
- **THEN** the migration service exits unsuccessfully
- **AND** the API does not start
- **AND** the failure is visible in Compose service output
