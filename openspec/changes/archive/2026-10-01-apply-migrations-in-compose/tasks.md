# Tasks

## 1. Migrations container

- [x] 1.1 Add a container build target for `OrderHub.Infrastructure.Migrations` and verify it produces a runnable migration executable from the current source revision.
- [x] 1.2 Add the one-shot migrations service with the API database connection settings and a healthy-PostgreSQL dependency; verify `docker compose config` resolves both services and their dependencies.

## 2. Startup ordering and documentation

- [x] 2.1 Make the API wait for successful migration completion while preserving the Visual Studio Debug/Fast Mode startup path; verify the Compose project configuration and debugger attachment.
- [x] 2.2 Document normal startup and migration-failure diagnostics, then verify the documented commands against the Compose files.

## 3. Integration verification

- [x] 3.1 Start Compose against a fresh database and verify migrations complete before API health becomes ready.
- [x] 3.2 Restart against an already-current database and verify the migration step exits successfully without schema changes.
- [x] 3.3 Simulate a migration failure and verify Compose exposes the migration error and does not start the API.
