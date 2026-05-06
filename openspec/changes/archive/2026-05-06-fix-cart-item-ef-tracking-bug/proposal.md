## Why

`CartService.AddItemAsync` throws `DbUpdateConcurrencyException` every time a new product is added to a cart because EF Core generates `UPDATE` instead of `INSERT` for the new `CartItem`, finding 0 matching rows. This breaks the core add-to-cart flow for all users.

## What Changes

- Add `AddCartItemAsync(CartItem item)` to `ICartRepository` and `CartRepository`, which uses `context.CartItems.Add(item)` to explicitly register the entity as `Added` in the change tracker.
- Refactor `CartService.AddItemAsync` to call the new repository method instead of mutating `cart.CartItems` directly, eliminating the ambiguous entity-state issue.
- Remove manual `Id = Guid.NewGuid()` assignment from `CartItem` construction — let EF Core generate the value via `ValueGeneratedOnAdd`, removing the non-default key confusion.

## Capabilities

### New Capabilities
<!-- none -->

### Modified Capabilities
- `cart-management`: Implementation-only fix — AddItem now correctly INSERTs new CartItems. No spec-level behavior change.

## Impact

- `OrderSystem.Application/Interfaces/ICartRepository.cs` — new method signature
- `OrderSystem.Infrastructure/Repositories/CartRepository.cs` — new method implementation
- `OrderSystem.Application/Services/CartService.cs` — `AddItemAsync` refactored
- `OrderSystem.Domain/Entities/CartItem.cs` — remove manual `Id` initialization if needed
- No API contract changes, no migration needed
