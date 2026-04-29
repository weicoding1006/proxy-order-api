## Why

The project currently uses Scalar as the interactive API documentation UI, but the team wants to switch to the more widely-adopted Swagger UI (via Swashbuckle) for consistency with tooling, familiarity, and broader ecosystem support.

## What Changes

- Remove `Scalar.AspNetCore` NuGet package from `OrderSystem.Api.csproj`
- Add `Swashbuckle.AspNetCore` NuGet package
- Replace `using Scalar.AspNetCore` with Swashbuckle service/middleware calls in `Program.cs`
- Replace `app.MapScalarApiReference()` with `app.UseSwagger()` and `app.UseSwaggerUI()` in the development block
- Replace `builder.Services.AddOpenApi()` with `builder.Services.AddEndpointsApiExplorer()` and `builder.Services.AddSwaggerGen()` including JWT Bearer security scheme configuration
- **BREAKING**: API docs URL changes from `/scalar/v1` to `/swagger`

## Capabilities

### New Capabilities

- `swagger-api-docs`: Serve Swagger UI at `/swagger` in development, with JWT Bearer auth support so developers can test protected endpoints directly from the browser.

### Modified Capabilities

- `scalar-api-docs`: Requirements replaced — Scalar UI is removed entirely and superseded by `swagger-api-docs`.

## Impact

- `OrderSystem.Api/Program.cs`: service registration and middleware pipeline changes
- `OrderSystem.Api/OrderSystem.Api.csproj`: package reference swap (Scalar out, Swashbuckle in)
- `openspec/specs/scalar-api-docs/spec.md`: spec is superseded by new `swagger-api-docs` spec
