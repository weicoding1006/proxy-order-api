## 1. Add NuGet Packages

- [x] 1.1 Add `Microsoft.AspNetCore.Identity.EntityFrameworkCore` to `OrderSystem.Api.csproj`
- [x] 1.2 Add `Scalar.AspNetCore` to `OrderSystem.Api.csproj`

## 2. Update OrderDbContext

- [x] 2.1 Change `OrderDbContext` base class from `DbContext` to `IdentityDbContext<IdentityUser>` in `Data/OrderDbContext.cs`
- [x] 2.2 Add required `using` directives (`Microsoft.AspNetCore.Identity`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore`)
- [x] 2.3 Verify the project compiles without errors after the base class change

## 3. Register Identity in Program.cs

- [x] 3.1 Add `AddIdentityCore<IdentityUser>().AddEntityFrameworkStores<OrderDbContext>().AddApiEndpoints()` service registration
- [x] 3.2 Configure `IdentityOptions` password requirements if needed (relax in Development)

## 4. Map Identity API Endpoints

- [x] 4.1 Add `app.MapIdentityApi<IdentityUser>()` in `Program.cs` to auto-generate all auth endpoints (register, login, refresh, manage, etc.)
- [ ] 4.2 Verify endpoints appear in Scalar UI

## 5. Integrate Scalar

- [x] 5.1 Replace `app.MapOpenApi()` UI call with `app.MapScalarApiReference()` inside the `IsDevelopment()` block in `Program.cs`
- [ ] 5.2 Verify Scalar UI loads at `/scalar/v1` in development and shows all endpoints including Identity ones

## 6. EF Core Migration

- [x] 6.1 Run `dotnet ef migrations add AddIdentitySchema` and verify the generated migration contains all 7 Identity tables
- [x] 6.2 Inspect the generated migration file — confirm no accidental drops/alters on existing tables
- [x] 6.3 Run `dotnet ef database update` to apply the migration
- [x] 6.4 Verify `__EFMigrationsHistory` contains both `InitialCreate` and `AddIdentitySchema`

## 7. Roles Controller (Manual)

- [x] 7.1 Create `Controllers/RolesController.cs` with `[ApiController]` and `[Route("api/roles")]`
- [x] 7.2 Implement `GET /api/roles` returning all roles via `RoleManager<IdentityRole>`
- [x] 7.3 Implement `POST /api/roles` creating a new role via `RoleManager.CreateAsync`
- [x] 7.4 Implement `POST /api/roles/{roleId}/users` assigning a user to a role via `UserManager.AddToRoleAsync`
- [x] 7.5 Implement `DELETE /api/roles/{roleId}/users/{userId}` removing a user from a role via `UserManager.RemoveFromRoleAsync`

## 8. Update Specs

- [x] 8.1 Update `openspec/specs/efcore-dbcontext/spec.md` to reflect the `IdentityDbContext` requirement
- [x] 8.2 Update `openspec/specs/efcore-migrations/spec.md` to reflect the `AddIdentitySchema` migration requirement
