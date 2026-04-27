## 1. Install NuGet Packages

- [x] 1.1 Add `Npgsql.EntityFrameworkCore.PostgreSQL` package to `OrderSystem.Api.csproj`
- [x] 1.2 Add `Microsoft.EntityFrameworkCore.Design` package (PrivateAssets=all) to `OrderSystem.Api.csproj`
- [x] 1.3 Add `Microsoft.EntityFrameworkCore.Tools` package (PrivateAssets=all) to `OrderSystem.Api.csproj`
- [x] 1.4 Verify `dotnet restore` completes without errors

## 2. Configure Connection String

- [x] 2.1 Add `ConnectionStrings:DefaultConnection` placeholder to `appsettings.json`
- [x] 2.2 Add real local PostgreSQL connection string to `appsettings.Development.json` (ensure this file is in `.gitignore` or use user-secrets)
- [x] 2.3 Verify connection string is overridable via `ConnectionStrings__DefaultConnection` environment variable

## 3. Create OrderDbContext

- [x] 3.1 Create `Data/` folder under `OrderSystem.Api`
- [x] 3.2 Create `Data/OrderDbContext.cs` inheriting from `DbContext` with a constructor accepting `DbContextOptions<OrderDbContext>`
- [x] 3.3 Confirm context has no `DbSet` properties yet (baseline scaffold — entities added in future features)

## 4. Register DbContext in DI

- [x] 4.1 In `Program.cs`, call `builder.Services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")))` before `builder.Build()`
- [ ] 4.2 Confirm the app starts without errors when the connection string is present
- [ ] 4.3 Confirm startup throws an informative exception when `ConnectionStrings:DefaultConnection` is missing

## 5. Set Up Migrations Tooling

- [x] 5.1 Install `dotnet-ef` global tool if not already present: `dotnet tool install --global dotnet-ef`
- [x] 5.2 Run `dotnet ef migrations add InitialCreate --project OrderSystem.Api` to generate the initial migration
- [x] 5.3 Verify `Migrations/` folder is created with `<timestamp>_InitialCreate.cs` and `OrderDbContextModelSnapshot.cs`

## 6. Apply and Verify Migration

- [ ] 6.1 Start a local PostgreSQL instance (Docker: `docker run -d -e POSTGRES_PASSWORD=postgres -p 5432:5432 postgres:17`)
- [ ] 6.2 Run `dotnet ef database update --project OrderSystem.Api` and confirm it exits with code 0
- [ ] 6.3 Connect to PostgreSQL and verify `__EFMigrationsHistory` table exists with one row for `InitialCreate`

## 7. Commit and Validate

- [ ] 7.1 Commit all source changes: updated `.csproj`, `appsettings.json`, `Program.cs`, `Data/OrderDbContext.cs`, and the entire `Migrations/` folder
- [ ] 7.2 Verify `dotnet build` succeeds on a clean clone
- [ ] 7.3 Verify `dotnet ef migrations list` shows `InitialCreate` as applied
