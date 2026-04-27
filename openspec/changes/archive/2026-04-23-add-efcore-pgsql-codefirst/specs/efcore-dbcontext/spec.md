## ADDED Requirements

### Requirement: DbContext is registered in DI
The application SHALL register `OrderDbContext` with the ASP.NET Core DI container using `AddDbContext<OrderDbContext>` and the Npgsql provider, reading the connection string from `ConnectionStrings:DefaultConnection` in `IConfiguration`.

#### Scenario: Successful DI registration
- **WHEN** the application starts with a valid `ConnectionStrings:DefaultConnection` value in configuration
- **THEN** `OrderDbContext` SHALL be resolvable from the DI container without error

#### Scenario: Missing connection string
- **WHEN** `ConnectionStrings:DefaultConnection` is absent from configuration
- **THEN** the application SHALL throw an `InvalidOperationException` at startup with a message indicating the missing connection string

### Requirement: Connection string is environment-overridable
The application SHALL support overriding the database connection string via the environment variable `ConnectionStrings__DefaultConnection` without code changes.

#### Scenario: Environment variable overrides appsettings
- **WHEN** `ConnectionStrings__DefaultConnection` is set as an environment variable
- **THEN** EF Core SHALL use that value as the connection string, ignoring any value in `appsettings.json`

### Requirement: DbContext exposes typed DbSets
`OrderDbContext` SHALL inherit from `DbContext` and declare typed `DbSet<TEntity>` properties for each domain entity registered in the model.

#### Scenario: Initial empty context compiles
- **WHEN** `OrderDbContext` is defined with no `DbSet` properties (baseline scaffold)
- **THEN** the project SHALL compile without errors and EF Core tooling SHALL be able to resolve the context

### Requirement: Sensitive data is not logged
`OrderDbContext` SHALL NOT enable EF Core sensitive data logging in any environment (connection strings and parameter values must not appear in logs).

#### Scenario: Query executed with parameters
- **WHEN** a parameterized query is executed
- **THEN** the EF Core log output SHALL NOT include the raw parameter values
