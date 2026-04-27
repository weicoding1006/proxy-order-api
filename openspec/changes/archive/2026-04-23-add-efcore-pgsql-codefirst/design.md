## Context

`OrderSystem.Api` is a .NET 10 ASP.NET Core Web API with no persistence layer. The solution currently contains only a single controller and no NuGet packages beyond `Microsoft.AspNetCore.OpenApi`. We need to introduce a relational database layer using EF Core + PostgreSQL in a code-first style so that domain models drive the schema and changes are tracked through versioned migrations.

## Goals / Non-Goals

**Goals:**
- Install and configure `Npgsql.EntityFrameworkCore.PostgreSQL` as the EF Core provider
- Create `OrderDbContext` as the central unit-of-work registered in the DI container
- Establish the `dotnet ef` migrations workflow (add, apply, revert)
- Produce an initial migration from the baseline model

**Non-Goals:**
- Define domain entity models (done in subsequent features; this change only scaffolds the infrastructure)
- Repository pattern or unit-of-work abstraction layer on top of EF Core
- Multi-tenancy, read replicas, or connection pooling configuration
- Automated migration on startup (`Database.Migrate()` in `Program.cs`)

## Decisions

### 1. Npgsql.EntityFrameworkCore.PostgreSQL as the provider

**Choice:** `Npgsql.EntityFrameworkCore.PostgreSQL`  
**Why:** Official, actively maintained Npgsql provider; full EF Core feature parity; supports .NET 10 and PostgreSQL 15+.  
**Alternative considered:** Devart dotConnect — commercial license, ruled out.

### 2. Connection string via `IConfiguration` / `appsettings`

**Choice:** Read `ConnectionStrings:DefaultConnection` from `appsettings.json`, overridable via environment variable `ConnectionStrings__DefaultConnection`.  
**Why:** Standard ASP.NET Core convention; no secrets in source; works with Docker / Kubernetes environment variable injection and local `appsettings.Development.json` overrides.  
**Alternative considered:** Hardcoding in `Program.cs` — rejected (insecure, inflexible).

### 3. `DbContext` lives in `OrderSystem.Api` project (for now)

**Choice:** Place `Data/OrderDbContext.cs` inside the single API project.  
**Why:** The solution currently has one project. Extracting to a separate class library is premature and can be done when a second consumer (e.g., background worker) emerges.  
**Alternative considered:** Separate `OrderSystem.Infrastructure` project — valid long-term, deferred.

### 4. `dotnet-ef` global tool + `Microsoft.EntityFrameworkCore.Design` package

**Choice:** Developers run `dotnet ef migrations add <Name>` and `dotnet ef database update` from the CLI.  
**Why:** Standard EF Core tooling, no IDE dependency, works in CI pipelines.  
**Alternative considered:** EF Core Power Tools (VS extension) — not cross-platform, skipped.

## Risks / Trade-offs

| Risk | Mitigation |
|---|---|
| PostgreSQL not available in dev environment | Provide a `docker-compose.yml` snippet in the tasks file for a one-command local Postgres |
| EF Core 10 package availability | Npgsql 10.x targets EF Core 10; verify compatible version on NuGet before pinning |
| Migration drift if multiple devs add migrations simultaneously | Establish convention: always pull latest, apply pending migrations before adding new one |
| Startup failure on missing connection string | `AddDbContext` call will throw at DI resolution time; document required config key clearly |

## Migration Plan

1. Install packages via `dotnet add package`
2. Create `Data/OrderDbContext.cs` with empty `DbSet<>` placeholders
3. Register in `Program.cs` with `builder.Services.AddDbContext<OrderDbContext>(...)`
4. Add connection string to `appsettings.Development.json` (local only; never commit real credentials)
5. Run `dotnet ef migrations add InitialCreate` — creates `Migrations/` folder
6. Run `dotnet ef database update` against local PostgreSQL to verify migration applies cleanly
7. Commit all generated migration files alongside source changes

**Rollback:** `dotnet ef database update 0` drops all migrated schema; remove the `Migrations/` folder to reset completely.

## Open Questions

- What PostgreSQL version will be used in production? (affects Npgsql version pin)
- Will there be a CI step to verify migrations are up-to-date against the model?
