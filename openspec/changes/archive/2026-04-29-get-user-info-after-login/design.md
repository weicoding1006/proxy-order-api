## Context

The login endpoint (`POST /api/auth/login`) returns `{ token, expiresAt }`. The JWT contains `sub`, `email`, and `roles` claims, but **not** `firstName` or `lastName` — those are stored in `AspNetUsers`. Frontends need a reliable, up-to-date user profile without decoding the JWT client-side or adding more claims to every token.

## Goals / Non-Goals

**Goals:**
- Expose `GET /api/auth/me` that returns `{ id, email, firstName, lastName, roles }` for the currently authenticated user
- Keep the login response unchanged

**Non-Goals:**
- Embedding `firstName`/`lastName` into the JWT (increases token size for every request)
- Profile update (PATCH /me) — out of scope for this change
- Admin access to other users' profiles

## Decisions

**Single DB lookup via UserManager**
Reading `firstName` and `lastName` requires one `UserManager.FindByIdAsync(userId)` call using the `sub` claim from the JWT. This is acceptable — it's a single indexed PK lookup and avoids bloating JWT claims.

Alternative considered: Add `firstName`/`lastName` to JWT claims at login time. Rejected because stale name data would persist until token expiry, and it increases the JWT payload sent on every authenticated request.

**Action lives in AuthController**
`GET /api/auth/me` fits naturally alongside `register` and `login` in `AuthController`. No new controller needed.

**Roles from UserManager, not JWT claims**
`GetRolesAsync` returns authoritative roles; reading `ClaimTypes.Role` from the token would return roles as of token issuance. For a profile endpoint, authoritative data is preferred.

## Risks / Trade-offs

- [Extra DB call per `/me` request] → Mitigated by HTTP caching on the client (frontend can cache the result until logout); the endpoint is not called on every page navigation
- [UserManager.FindByIdAsync returns null if user is deleted post-login] → Return `404 Not Found` in that edge case
