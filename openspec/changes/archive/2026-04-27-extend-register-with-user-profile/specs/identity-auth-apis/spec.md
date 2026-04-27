## MODIFIED Requirements

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
