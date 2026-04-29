## 1. Application Layer — DTO

- [x] 1.1 Create `UserProfileResponse` DTO in `OrderSystem.Application/DTOs/Auth/` with properties: `Id`, `Email`, `FirstName`, `LastName`, `Roles`

## 2. API Layer — Controller Action

- [x] 2.1 Add `[HttpGet("me")]` action to `AuthController` decorated with `[Authorize]`
- [x] 2.2 Resolve `userId` from `User.FindFirstValue(ClaimTypes.NameIdentifier)` inside the action (ASP.NET Core JWT middleware maps `sub` → `ClaimTypes.NameIdentifier`; using `JwtRegisteredClaimNames.Sub` returns null)
- [x] 2.3 Call `userManager.FindByIdAsync(userId)` and return `404 Not Found` if user is null
- [x] 2.4 Call `userManager.GetRolesAsync(user)` to get current roles
- [x] 2.5 Return `200 OK` with a `UserProfileResponse` populated from the user entity and roles list

## 3. Verification

- [x] 3.1 Use Swagger UI: login to get a token, authorize, then call `GET /api/auth/me` and verify the response body matches the spec
- [x] 3.2 Verify that calling `GET /api/auth/me` without a token returns `401 Unauthorized`
