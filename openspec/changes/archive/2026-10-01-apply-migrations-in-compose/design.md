# Design

## Context

See `proposal.md` for motivation. The existing `OrderHub.Infrastructure.Migrations` executable calls EF Core `MigrateAsync` and requires a PostgreSQL connection string. The API currently waits for PostgreSQL health but not for schema migration.

## Goals / Non-Goals

**Goals:**
- Run the dedicated migrations executable once as part of Compose startup.
- Preserve Visual Studio Fast Mode and CLI startup behavior.
- Keep migration execution outside the API process.

**Non-Goals:**
- Running migrations automatically in production deployment systems outside the repository's Compose environment.
- Replacing EF Core migrations or changing schema ownership.

## Decisions

- Add a one-shot `migrations` Compose service that builds/publishes the existing migration project and receives the same database connection configuration as the API. This reuses the existing migration entry point rather than adding schema logic to the API.
- Make `api` depend on `migrations` with `service_completed_successfully`; the migrations service itself waits for `postgres` to become healthy. This makes failure prevent an incompatible API startup and ensures no parallel API/migration race.
- Keep the service in the base Compose file so both `docker compose up --build` and Visual Studio's Compose project see the prerequisite. Verify the Visual Studio Fast Mode profile does not override this dependency; if it does, adjust the project Compose configuration without changing the API debugger mode.
- Do not bind the migration container to API Debug output; it is an independent one-shot executable built from the same source revision.

## Risks / Trade-offs

- [Risk] Visual Studio Fast Mode may treat service dependencies differently → validate the project profile and retain the existing API Debug behavior.
- [Risk] A migration failure now blocks API startup → expose the migration container's exit code and logs as the actionable diagnostic.
- [Risk] Concurrent Compose projects could attempt migrations simultaneously → rely on the existing EF migration history and database-level migration behavior; do not run multiple migration services in one Compose project.

## Migration Plan

No data migration is introduced. On startup, the one-shot service applies pending migrations. Rollback consists of reverting the Compose wiring; database schema rollback remains governed by the existing migration policy and is not automatic.
