## REMOVED Requirements

### Requirement: Scalar UI is served in development
**Reason**: Replaced by Swagger UI (`swagger-api-docs` capability). Scalar is being removed from the project entirely.
**Migration**: Use Swagger UI at `/swagger` instead of `/scalar/v1`.

### Requirement: Scalar is registered via MapScalarApiReference
**Reason**: Scalar package is removed; Swashbuckle middleware replaces it.
**Migration**: `app.MapScalarApiReference()` is replaced by `app.UseSwagger()` and `app.UseSwaggerUI()`.

### Requirement: Auth endpoints are visible in Scalar
**Reason**: Superseded by `swagger-api-docs` — Bearer token auth is now configured via Swagger UI's Authorize button.
**Migration**: See `swagger-api-docs` spec for Swagger JWT auth requirements.
