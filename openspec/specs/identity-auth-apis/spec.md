### Requirement: User registration endpoint
The application SHALL expose `POST /api/auth/register` that accepts `{ email, password, firstName, lastName }` and creates a new `ApplicationUser` via `UserManager`. `firstName` and `lastName` are required and SHALL be persisted to `AspNetUsers`. On success it SHALL return `201 Created`. On failure it SHALL return `400 Bad Request` with the list of Identity errors.

#### Scenario: Successful registration
- **WHEN** `POST /api/auth/register` is called with a valid `email`, `password`, `firstName`, and `lastName`
- **THEN** the response SHALL be `201 Created`
- **AND** the user SHALL exist in `AspNetUsers` with `FirstName` and `LastName` populated

#### Scenario: Duplicate email rejected
- **WHEN** `POST /api/auth/register` is called with an email that already exists
- **THEN** the response SHALL be `400 Bad Request` with a body describing the duplicate email error

#### Scenario: Invalid password rejected
- **WHEN** `POST /api/auth/register` is called with a password that violates the configured policy
- **THEN** the response SHALL be `400 Bad Request` with a body listing all password policy violations

### Requirement: User login endpoint returns JWT
The application SHALL expose `POST /api/auth/login` that accepts `{ email, password }`, validates credentials via `SignInManager`, and on success returns `200 OK` with `{ token, expiresAt }` where `token` is a signed JWT containing the user's id, email, and roles as claims.

#### Scenario: Successful login
- **WHEN** `POST /api/auth/login` is called with valid credentials
- **THEN** the response SHALL be `200 OK` with a `token` field containing a valid JWT
- **AND** the JWT SHALL include claims for `sub` (user id), `email`, and any assigned roles

#### Scenario: Wrong password returns 401
- **WHEN** `POST /api/auth/login` is called with a correct email but wrong password
- **THEN** the response SHALL be `401 Unauthorized`

#### Scenario: Unknown email returns 401
- **WHEN** `POST /api/auth/login` is called with an email that does not exist
- **THEN** the response SHALL be `401 Unauthorized` (same response as wrong password to avoid user enumeration)

### Requirement: Role list and create endpoints
The application SHALL expose `GET /api/roles` (returns all roles) and `POST /api/roles` (creates a role) accessible only to authenticated users with the `Admin` role.

#### Scenario: List roles as admin
- **WHEN** an authenticated admin calls `GET /api/roles`
- **THEN** the response SHALL be `200 OK` with a JSON array of role objects (`{ id, name }`)

#### Scenario: Create role as admin
- **WHEN** an authenticated admin calls `POST /api/roles` with `{ name: "Manager" }`
- **THEN** the response SHALL be `201 Created` and the role SHALL exist in `AspNetRoles`

#### Scenario: Non-admin cannot create roles
- **WHEN** an authenticated non-admin user calls `POST /api/roles`
- **THEN** the response SHALL be `403 Forbidden`

### Requirement: Assign and remove user roles
The application SHALL expose `POST /api/roles/{roleId}/users` (assign a user to a role) and `DELETE /api/roles/{roleId}/users/{userId}` (remove a user from a role), accessible only to admin users.

#### Scenario: Assign user to role
- **WHEN** an admin calls `POST /api/roles/{roleId}/users` with `{ userId }`
- **THEN** the response SHALL be `200 OK`
- **AND** the user SHALL have the specified role in `AspNetUserRoles`

#### Scenario: Remove user from role
- **WHEN** an admin calls `DELETE /api/roles/{roleId}/users/{userId}`
- **THEN** the response SHALL be `204 No Content`
- **AND** the user SHALL no longer have that role

#### Scenario: Assign non-existent user returns 404
- **WHEN** an admin calls `POST /api/roles/{roleId}/users` with a userId that does not exist
- **THEN** the response SHALL be `404 Not Found`

### Requirement: Protected endpoints require Bearer token
Endpoints decorated with `[Authorize]` SHALL return `401 Unauthorized` when called without a valid Bearer token, and `403 Forbidden` when called with a valid token that lacks the required role.

#### Scenario: Missing token returns 401
- **WHEN** a request is made to a protected endpoint with no `Authorization` header
- **THEN** the response SHALL be `401 Unauthorized`

#### Scenario: Valid token with wrong role returns 403
- **WHEN** a request is made to an admin-only endpoint with a valid JWT that has no admin role claim
- **THEN** the response SHALL be `403 Forbidden`
