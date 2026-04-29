## Context

The API currently uses `Scalar.AspNetCore` (v2.14.3) as the interactive documentation UI, layered on top of ASP.NET Core's built-in `Microsoft.AspNetCore.OpenApi` (`AddOpenApi` / `MapOpenApi`). The goal is to replace Scalar with Swashbuckle's Swagger UI while retaining JWT Bearer authentication support in the UI.

Current stack:
- `builder.Services.AddOpenApi()` — generates the OpenAPI document
- `app.MapOpenApi()` — serves the raw JSON spec
- `app.MapScalarApiReference()` — serves Scalar UI at `/scalar/v1`

## Goals / Non-Goals

**Goals:**
- Serve Swagger UI at `/swagger` in development
- Support JWT Bearer token entry so protected endpoints can be tested from the browser
- Remove the `Scalar.AspNetCore` package entirely

**Non-Goals:**
- Changing any API behavior, routes, or response shapes
- Enabling Swagger UI in production (stays development-only)
- Migrating OpenAPI document customizations beyond auth scheme

## Decisions

### Decision: Use Swashbuckle.AspNetCore instead of NSwag

Swashbuckle is the most widely adopted Swagger library for ASP.NET Core, has the richest documentation, and integrates with the existing `AddEndpointsApiExplorer` pipeline. NSwag would work too but adds code-generation overhead that isn't needed here.

**Alternative considered**: Keep `Microsoft.AspNetCore.OpenApi` + add a lightweight Swagger UI HTML file — rejected because Swashbuckle already handles this cleanly with `UseSwaggerUI`.

### Decision: Replace AddOpenApi with AddSwaggerGen

`Microsoft.AspNetCore.OpenApi`'s `AddOpenApi` is a minimal built-in that Scalar consumed directly. Swashbuckle uses its own document generation via `AddSwaggerGen`, so `AddOpenApi` / `MapOpenApi` are removed to avoid duplicate doc endpoints.

### Decision: Configure SecurityDefinition + SecurityRequirement in AddSwaggerGen

JWT Bearer auth must be declared as a security definition inside `AddSwaggerGen` so that the Swagger UI renders the "Authorize" button. A global `SecurityRequirement` is added so all endpoints show the lock icon by default.

## Risks / Trade-offs

- **URL breaking change** → `/scalar/v1` no longer works; any bookmarks or docs pointing there break. Mitigation: noted as BREAKING in proposal; low risk since this is a dev-only UI.
- **Swashbuckle XML comments** → Without XML doc generation enabled, summary/description fields will be empty. Mitigation: acceptable for now; can be added later.

## Migration Plan

1. Remove `Scalar.AspNetCore` package reference from `.csproj`
2. Add `Swashbuckle.AspNetCore` package
3. Update `Program.cs`: swap service registrations and middleware
4. Delete or supersede `scalar-api-docs` spec
5. Verify Swagger UI loads at `https://localhost:<port>/swagger` in development
