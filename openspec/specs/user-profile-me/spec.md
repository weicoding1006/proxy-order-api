## ADDED Requirements

### Requirement: Get current user profile endpoint
The application SHALL expose `GET /api/auth/me` protected by `[Authorize]`. It SHALL resolve the user by the `sub` claim in the Bearer JWT and return `200 OK` with `{ id, email, firstName, lastName, roles }`. If the user no longer exists in the database, it SHALL return `404 Not Found`.

#### Scenario: Authenticated user receives profile
- **WHEN** a valid JWT Bearer token is supplied to `GET /api/auth/me`
- **THEN** the response SHALL be `200 OK`
- **AND** the body SHALL contain `id` (string), `email` (string), `firstName` (string), `lastName` (string), and `roles` (array of strings)

#### Scenario: Unauthenticated request is rejected
- **WHEN** `GET /api/auth/me` is called without an `Authorization` header
- **THEN** the response SHALL be `401 Unauthorized`

#### Scenario: Deleted user returns 404
- **WHEN** a valid JWT is presented but the corresponding user has been deleted from the database
- **THEN** the response SHALL be `404 Not Found`

#### Scenario: Roles array reflects current assignments
- **WHEN** an authenticated user whose roles have changed since token issuance calls `GET /api/auth/me`
- **THEN** the `roles` array SHALL reflect the user's current roles from the database, not the roles encoded in the JWT
