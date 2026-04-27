## Why

The current `OrderDbContext` inherits from plain `DbContext`, providing no built-in identity or permission management. Adopting `IdentityDbContext` unlocks ASP.NET Core Identity (users, roles, claims) without reinventing auth infrastructure, and Scalar replaces the default OpenAPI UI with a modern, interactive developer experience.

## What Changes

- Replace `OrderDbContext` base class from `DbContext` → `IdentityDbContext<IdentityUser>` (or a custom user type)
- Add NuGet packages: `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Scalar.AspNetCore`
- Generate a new EF Core migration that creates the 7 ASP.NET Identity tables (`AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, `AspNetUserClaims`, `AspNetRoleClaims`, `AspNetUserLogins`, `AspNetUserTokens`)
- Replace `app.MapOpenApi()` default UI with Scalar middleware
- Add auth-related API endpoints: register, login (JWT), logout, role management, and user-role assignment

## Capabilities

### New Capabilities
- `identity-dbcontext`: Migrate `OrderDbContext` to inherit from `IdentityDbContext`, registering identity services in DI and enabling Identity table generation via migration
- `scalar-api-docs`: Integrate Scalar as the OpenAPI UI, served at `/scalar/v1` in development
- `identity-auth-apis`: Expose REST endpoints for user registration, JWT-based login, logout, role CRUD, and assigning/removing roles from users

### Modified Capabilities
- `efcore-dbcontext`: The DbContext now inherits from `IdentityDbContext<IdentityUser>` instead of `DbContext`; DI registration must call `AddIdentity`/`AddDefaultIdentity` in addition to `AddDbContext`
- `efcore-migrations`: A new migration (`AddIdentitySchema`) must be generated and applied after the `IdentityDbContext` change; the existing `InitialCreate` migration is unaffected

## Impact

- **Code**: `OrderSystem.Api/Data/OrderDbContext.cs`, `Program.cs`
- **New files**: Auth controller(s), JWT configuration, optional custom `ApplicationUser` model
- **Database**: 7 new Identity tables created by migration
- **Dependencies**: `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Scalar.AspNetCore`, `Microsoft.AspNetCore.Authentication.JwtBearer`
- **API surface**: New `/api/auth/*` and `/api/roles/*` endpoints added; Scalar UI replaces default OpenAPI browser
