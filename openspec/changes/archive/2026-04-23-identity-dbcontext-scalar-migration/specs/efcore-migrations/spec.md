## ADDED Requirements

### Requirement: AddIdentitySchema migration generates Identity tables
A migration named `AddIdentitySchema` SHALL be generated after the `IdentityDbContext` change and committed to source control. This migration SHALL create all 7 ASP.NET Identity tables and SHALL NOT modify any tables created by earlier migrations.

#### Scenario: Migration is generated from updated context
- **WHEN** a developer runs `dotnet ef migrations add AddIdentitySchema` after switching to `IdentityDbContext<IdentityUser>`
- **THEN** the `Migrations/` folder SHALL contain `<timestamp>_AddIdentitySchema.cs` and the updated snapshot file

#### Scenario: Migration applies cleanly after InitialCreate
- **WHEN** `dotnet ef database update` is run against a database that already has `InitialCreate` applied
- **THEN** `AddIdentitySchema` SHALL apply without errors
- **AND** `__EFMigrationsHistory` SHALL contain rows for both `InitialCreate` and `AddIdentitySchema`

#### Scenario: Migration does not alter pre-existing tables
- **WHEN** `AddIdentitySchema` is applied
- **THEN** no `ALTER TABLE` or `DROP TABLE` statements SHALL target tables that were created by `InitialCreate`

## MODIFIED Requirements

### Requirement: Migrations can be rolled back
The migration system SHALL support rolling back the database to a previous migration using `dotnet ef database update <TargetMigration>`. Rolling back to `InitialCreate` SHALL remove all Identity tables without affecting the baseline schema. Rolling back to `0` SHALL remove all EF-managed tables.

#### Scenario: Rollback to InitialCreate
- **WHEN** `dotnet ef database update InitialCreate` is executed after `AddIdentitySchema` has been applied
- **THEN** all 7 Identity tables SHALL be dropped
- **AND** `__EFMigrationsHistory` SHALL contain only the row for `InitialCreate`

#### Scenario: Rollback to migration zero
- **WHEN** `dotnet ef database update 0` is executed
- **THEN** all EF-managed tables SHALL be dropped and `__EFMigrationsHistory` SHALL be empty
