## 1. Package Swap

- [x] 1.1 Remove `Scalar.AspNetCore` package reference from `OrderSystem.Api/OrderSystem.Api.csproj`
- [x] 1.2 Add `Swashbuckle.AspNetCore` package reference to `OrderSystem.Api/OrderSystem.Api.csproj` (run `dotnet add package Swashbuckle.AspNetCore`)

## 2. Program.cs — Service Registration

- [x] 2.1 Remove `using Scalar.AspNetCore;` from `Program.cs`
- [x] 2.2 Replace `builder.Services.AddOpenApi()` with `builder.Services.AddEndpointsApiExplorer()` and `builder.Services.AddSwaggerGen(...)` including JWT `SecurityDefinition` and global `SecurityRequirement`

## 3. Program.cs — Middleware Pipeline

- [x] 3.1 Inside the `if (app.Environment.IsDevelopment())` block, replace `app.MapOpenApi()` and `app.MapScalarApiReference()` with `app.UseSwagger()` and `app.UseSwaggerUI()`

## 4. Verification

- [ ] 4.1 Run the application in Development and confirm Swagger UI loads at `/swagger`
- [ ] 4.2 Confirm the "Authorize" button is present and a Bearer token can be entered
- [ ] 4.3 Test a protected endpoint via Swagger UI to confirm the `Authorization` header is sent
- [ ] 4.4 Confirm `/scalar/v1` returns 404 (Scalar is gone)
