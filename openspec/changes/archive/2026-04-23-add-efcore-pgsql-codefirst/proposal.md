## Why

The project currently has no persistence layer. Adding EF Core with PostgreSQL via a code-first approach gives the team a type-safe ORM, a schema that evolves with the domain models, and a versioned migration history that can be applied reliably across all environments.

## What Changes

- Add NuGet packages: `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore.Tools`
- Introduce a `DbContext` class (`OrderDbContext`) wired into the DI container
- Add connection string configuration in `appsettings.json` / `appsettings.Development.json`
- Set up the EF Core migrations infrastructure (`dotnet ef migrations` tooling)
- Create an initial migration to establish the baseline schema

## Capabilities

### New Capabilities

- `efcore-dbcontext`: Define `OrderDbContext`, register it with DI, and configure the Npgsql provider with connection-string support
- `efcore-migrations`: Establish the EF Core migrations workflow (add, apply, rollback) and generate the initial migration

### Modified Capabilities

_(none — this is a greenfield persistence layer addition)_

## Impact

- **`OrderSystem.Api.csproj`**: four new `<PackageReference>` entries
- **`appsettings.json` / `appsettings.Development.json`**: new `ConnectionStrings:DefaultConnection` key
- **`Program.cs`**: `AddDbContext<OrderDbContext>` registration
- **New files**: `Data/OrderDbContext.cs`, `Migrations/` folder (auto-generated)
- **External dependency**: a running PostgreSQL instance (local Docker or hosted) is required to apply migrations
