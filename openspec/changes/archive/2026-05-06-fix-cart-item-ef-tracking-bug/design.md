## Context

`CartService.AddItemAsync` creates a new `CartItem` with `Id = Guid.NewGuid()` and adds it to a tracked `Cart`'s navigation collection. EF Core / Npgsql treats non-default Guid keys on entities that enter the change tracker through a navigation collection add as potentially pre-existing, generating `UPDATE` instead of `INSERT`. The update hits 0 rows and throws `DbUpdateConcurrencyException`.

The secondary factor is the save-then-refetch cycle for new carts: `AddAsync` calls `SaveChangesAsync`, then `GetByUserIdAsync` returns the same tracked instance from the identity map. Adding a `CartItem` to this instance in a subsequent operation is where the tracker state gets confused.

## Goals / Non-Goals

**Goals:**
- Eliminate the `DbUpdateConcurrencyException` on `AddItemAsync`
- Guarantee EF Core always issues `INSERT` for new `CartItem` entities
- Keep the fix minimal and localized; avoid broad architectural changes

**Non-Goals:**
- Refactor the entire CartRepository pattern
- Add optimistic concurrency tokens to `CartItem`
- Change the public API or DTO contracts

## Decisions

### Decision 1: Add `AddCartItemAsync` to `ICartRepository` / `CartRepository`

**Chosen**: Expose `Task<CartItem> AddCartItemAsync(CartItem item)` that calls `context.CartItems.Add(item); await context.SaveChangesAsync(); return item;`.

`context.CartItems.Add()` explicitly sets `EntityState.Added`, bypassing any change-tracker inference based on key value. This is the most direct, EF-idiomatic fix.

**Alternative considered**: Call `context.Entry(cartItem).State = EntityState.Added` in the service layer. Rejected — leaks EF internals into the Application layer.

### Decision 2: Remove manual `Id` assignment on `CartItem`

**Chosen**: Do not set `Id = Guid.NewGuid()` in `CartService`. Let EF Core generate the Guid client-side via `ValueGeneratedOnAdd` (default for Guid PKs). After `context.CartItems.Add(item)` the Guid is generated before the INSERT.

**Alternative considered**: Keep manual assignment and rely solely on the explicit `Add` call. This works but retains the non-default-key smell; omitting the assignment is cleaner.

### Decision 3: Keep the re-fetch after save

The re-fetch (`GetByUserIdAsync` after mutation) populates the `Product` navigation property for the response DTO. This is retained unchanged — it is correct behavior, not the source of the bug.

## Risks / Trade-offs

- [Risk] Other callers could still add to `cart.CartItems` directly → Mitigation: document the repository method as the only sanctioned insert path; the service is the only caller today.
- [Risk] EF Core generates a different Guid than expected in logs → not a risk for correctness, only minor observability impact.

## Migration Plan

No database migration required. Changes are purely in application and infrastructure code. Deploy as a normal code update; rollback by reverting the three affected files.
