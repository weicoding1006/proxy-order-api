### Requirement: OrderDbContext inherits from IdentityDbContext
`OrderDbContext` SHALL inherit from `IdentityDbContext<IdentityUser>` (from `Microsoft.AspNetCore.Identity.EntityFrameworkCore`) instead of plain `DbContext`, enabling EF Core to generate the ASP.NET Identity schema.

#### Scenario: Context resolves with Identity base
- **WHEN** the application starts and `OrderDbContext` is resolved from DI
- **THEN** the context SHALL be an instance of `IdentityDbContext<IdentityUser>` and SHALL include all Identity-related `DbSet` properties (`Users`, `Roles`, etc.)

### Requirement: Identity services registered in DI
The application SHALL call `AddIdentityCore<IdentityUser, IdentityRole>()` chained with `.AddEntityFrameworkStores<OrderDbContext>().AddApiEndpoints()` during service registration, so that `UserManager<IdentityUser>`, `RoleManager<IdentityRole>` are resolvable from the DI container.

#### Scenario: UserManager is injectable
- **WHEN** a controller or service requests `UserManager<IdentityUser>` from DI
- **THEN** the container SHALL resolve the instance without error

#### Scenario: RoleManager is injectable
- **WHEN** a controller or service requests `RoleManager<IdentityRole>` from DI
- **THEN** the container SHALL resolve the instance without error

### Requirement: Identity tables exist after migration
After running the `AddIdentitySchema` migration, the database SHALL contain exactly the following tables: `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, `AspNetUserClaims`, `AspNetRoleClaims`, `AspNetUserLogins`, `AspNetUserTokens`.

#### Scenario: Identity tables are created by migration
- **WHEN** `dotnet ef database update` is run against a database that has only `InitialCreate` applied
- **THEN** all 7 ASP.NET Identity tables SHALL be present in the database
- **AND** `__EFMigrationsHistory` SHALL contain a row for `AddIdentitySchema`

### Requirement: Password policy is configurable per environment
The application SHALL configure `IdentityOptions` to allow relaxed password requirements in development (e.g., `RequireDigit = false`, minimum length 6) while enforcing strict defaults in production.

#### Scenario: Weak password accepted in development
- **WHEN** the application runs in the `Development` environment
- **AND** a user registers with a password of 6 plain characters
- **THEN** the registration SHALL succeed without password policy errors

#### Scenario: Strict policy enforced in production
- **WHEN** the application runs in the `Production` environment
- **AND** a user registers with a password shorter than 8 characters or missing required character types
- **THEN** the registration SHALL return a 400 response listing password policy violations
