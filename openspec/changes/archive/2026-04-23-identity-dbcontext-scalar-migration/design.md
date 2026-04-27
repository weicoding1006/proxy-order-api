## Context

The project is an ASP.NET Core 10 Web API (`OrderSystem.Api`) backed by PostgreSQL via EF Core + Npgsql. The current `OrderDbContext` is a thin wrapper around `DbContext` with no entities and a single `InitialCreate` migration. There is no authentication, no user model, and the OpenAPI UI is the default `MapOpenApi()` endpoint.

The change introduces ASP.NET Core Identity (managed via `IdentityDbContext`) for user/role/claim management, integrates Scalar as the API documentation UI, and exposes auth-related REST endpoints protected by JWT.

## Goals / Non-Goals

**Goals:**
- `OrderDbContext` inherits from `IdentityDbContext<IdentityUser>` (or `ApplicationUser`)
- ASP.NET Core Identity services registered in DI with password/lockout defaults
- New EF Core migration (`AddIdentitySchema`) creates the 7 Identity tables
- Scalar served at `/scalar/v1` replacing the raw OpenAPI JSON endpoint as the developer UI
- JWT Bearer authentication wired up with configurable secret/issuer/audience from `appsettings`
- Auth endpoints: `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/logout`
- Role endpoints: `GET/POST /api/roles`, `DELETE /api/roles/{id}`, `POST /api/roles/{id}/users`, `DELETE /api/roles/{id}/users/{userId}`

**Non-Goals:**
- OAuth2 / external login providers (Google, GitHub, etc.)
- Refresh token rotation
- Email confirmation or password reset flows
- Fine-grained claim/policy-based authorization on existing order endpoints (deferred)
- UI / frontend — API only

## Decisions

### 1. `IdentityDbContext<IdentityUser>` vs. custom `ApplicationUser`
**Decision:** Start with `IdentityDbContext<IdentityUser>` using the default `IdentityUser`.  
**Rationale:** No custom profile fields are needed now. `ApplicationUser : IdentityUser` can be introduced later as a non-breaking migration if custom fields are required.  
**Alternative considered:** Custom `ApplicationUser` upfront — adds migration complexity with no present benefit.

### 2. JWT Bearer vs. ASP.NET Core Identity Cookies
**Decision:** JWT Bearer tokens.  
**Rationale:** This is a headless API consumed by external clients; stateless JWTs are the natural fit. Cookie auth requires same-origin browser sessions.  
**Alternative considered:** Cookie auth — ruled out for API-first design.

### 3. Scalar vs. Swashbuckle
**Decision:** Use `Scalar.AspNetCore` with the existing `AddOpenApi()` pipeline.  
**Rationale:** The project already calls `AddOpenApi()` (ASP.NET Core's built-in OpenAPI generator). Scalar wraps the generated JSON with a rich UI via one `MapScalarApiReference()` call — zero schema duplication.  
**Alternative considered:** Swashbuckle — requires replacing the built-in OpenAPI pipeline; more configuration overhead.

### 4. Migration strategy
**Decision:** Add a new migration `AddIdentitySchema` on top of the existing `InitialCreate`.  
**Rationale:** `InitialCreate` represents the baseline and must remain in history. The identity schema is purely additive.  
**Alternative considered:** Delete `InitialCreate` and recreate — breaks any deployed environments that already have `InitialCreate` in `__EFMigrationsHistory`.

### 5. Role management granularity
**Decision:** Expose basic CRUD for roles and user-role assignments via `RoleManager<IdentityRole>` and `UserManager<IdentityUser>`.  
**Rationale:** Satisfies the immediate need (listing, creating, assigning roles) without over-engineering a permissions engine.

## Risks / Trade-offs

- **Secret key management** → JWT signing key stored in `appsettings` is insecure in production. Mitigation: document that `JwtSettings:Secret` must be injected via environment variable / secrets manager in non-development environments.
- **`IdentityDbContext` adds 7 tables** → slight schema bloat if Identity is later removed. Mitigation: clean removal is a single migration rollback.
- **No refresh tokens** → access tokens expire and clients must re-login. Mitigation: set a reasonable expiry (60 min default) and document; refresh tokens are a future iteration.
- **Password policy defaults** → ASP.NET Identity defaults require digits, uppercase, symbols; may frustrate dev/test. Mitigation: relax defaults in development via `IdentityOptions` configuration, keep strict in production.

## Migration Plan

1. Add NuGet packages (`Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Scalar.AspNetCore`, `Microsoft.AspNetCore.Authentication.JwtBearer`)
2. Create `ApplicationUser : IdentityUser` (optional — keep as `IdentityUser` initially)
3. Update `OrderDbContext` to extend `IdentityDbContext<IdentityUser>`
4. Register `AddIdentity` + JWT Bearer in `Program.cs`
5. Run `dotnet ef migrations add AddIdentitySchema` → verify generated migration contains all 7 Identity tables
6. Run `dotnet ef database update`
7. Replace `app.MapOpenApi()` UI with Scalar registration
8. Implement `AuthController` and `RolesController`
9. Smoke-test register → login → use JWT on a protected endpoint

**Rollback:** `dotnet ef database update InitialCreate` reverts the identity tables. Revert code changes via git.

## Open Questions

- Should `ApplicationUser` with custom fields (e.g., `FullName`, `TenantId`) be introduced now or deferred?
- Is role-based authorization needed on existing endpoints (e.g., only admins can CRUD orders) in this iteration?
- JWT expiry duration — default 60 minutes acceptable?
