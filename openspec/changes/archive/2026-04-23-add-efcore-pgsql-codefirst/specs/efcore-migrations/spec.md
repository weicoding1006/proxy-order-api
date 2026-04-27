## ADDED Requirements

### Requirement: EF Core migrations tooling is available
The project SHALL reference `Microsoft.EntityFrameworkCore.Design` as a development dependency so that `dotnet ef migrations add` and `dotnet ef database update` commands can be executed from the CLI.

#### Scenario: Migrations tool executes successfully
- **WHEN** a developer runs `dotnet ef migrations list` in the project directory
- **THEN** the command SHALL exit with code 0 and list any applied or pending migrations

### Requirement: Initial migration establishes baseline schema
An initial migration named `InitialCreate` SHALL be generated from the baseline `OrderDbContext` model and committed to source control under the `Migrations/` folder.

#### Scenario: Initial migration is generated
- **WHEN** `dotnet ef migrations add InitialCreate` is run against the baseline context
- **THEN** the `Migrations/` folder SHALL contain `<timestamp>_InitialCreate.cs` and its snapshot file

#### Scenario: Initial migration applies to a clean database
- **WHEN** `dotnet ef database update` is run against a fresh PostgreSQL database
- **THEN** the migration SHALL apply without errors and the `__EFMigrationsHistory` table SHALL contain a row for `InitialCreate`

### Requirement: Migrations can be rolled back
The migration system SHALL support rolling back the database to a previous migration using `dotnet ef database update <TargetMigration>`.

#### Scenario: Rollback to migration zero
- **WHEN** `dotnet ef database update 0` is executed
- **THEN** all EF-managed tables SHALL be dropped and `__EFMigrationsHistory` SHALL be empty

### Requirement: Migration files are committed to source control
All files under `Migrations/` (including the model snapshot) SHALL be committed to the repository so that the migration history is shared across the team.

#### Scenario: New developer applies migrations from source
- **WHEN** a developer clones the repository and runs `dotnet ef database update`
- **THEN** all committed migrations SHALL be applied to their local database in order without manual steps
