## MODIFIED Requirements

### Requirement: DbContext is registered in DI
The application SHALL register `OrderDbContext` with the ASP.NET Core DI container using `AddDbContext<OrderDbContext>` and the Npgsql provider, reading the connection string from `ConnectionStrings:DefaultConnection` in `IConfiguration`. The registration SHALL be accompanied by `AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<OrderDbContext>()` so that Identity services share the same context and connection.

#### Scenario: Successful DI registration
- **WHEN** the application starts with a valid `ConnectionStrings:DefaultConnection` value in configuration
- **THEN** `OrderDbContext` SHALL be resolvable from the DI container without error

#### Scenario: Missing connection string
- **WHEN** `ConnectionStrings:DefaultConnection` is absent from configuration
- **THEN** the application SHALL throw an `InvalidOperationException` at startup with a message indicating the missing connection string

### Requirement: DbContext exposes typed DbSets
`OrderDbContext` SHALL inherit from `IdentityDbContext<IdentityUser>` (instead of `DbContext`) and SHALL declare typed `DbSet<TEntity>` properties for each domain entity registered in the model. The Identity-related `DbSet` properties (`Users`, `Roles`, etc.) are inherited from the base class and SHALL NOT be redeclared.

#### Scenario: Initial empty context compiles
- **WHEN** `OrderDbContext` is defined inheriting from `IdentityDbContext<IdentityUser>` with no additional `DbSet` properties
- **THEN** the project SHALL compile without errors and EF Core tooling SHALL be able to resolve the context
