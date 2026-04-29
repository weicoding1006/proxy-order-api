## Why

After a successful login, the frontend only receives a JWT token and expiry time but has no structured way to obtain the current user's profile (id, name, email, roles) without decoding the JWT client-side. A dedicated `/api/auth/me` endpoint lets the frontend fetch authoritative, up-to-date user info using the issued token.

## What Changes

- Add `GET /api/auth/me` endpoint that returns the authenticated user's profile
- The endpoint is protected by `[Authorize]` and reads identity from the JWT claims
- Response includes: `id`, `email`, `firstName`, `lastName`, `roles`

## Capabilities

### New Capabilities

- `user-profile-me`: Authenticated endpoint returning current user's id, email, firstName, lastName, and roles

### Modified Capabilities

- `identity-auth-apis`: Login response remains `{ token, expiresAt }` — no requirement change needed; the new `/me` endpoint is a separate concern

## Impact

- **New controller action** in `AuthController` (or equivalent)
- **New DTO** `UserProfileResponse` (or similar) in Application layer
- No database migrations required — data comes from existing `AspNetUsers` + `AspNetUserRoles`
- Frontend calls `GET /api/auth/me` with `Authorization: Bearer <token>` to populate user context after login
