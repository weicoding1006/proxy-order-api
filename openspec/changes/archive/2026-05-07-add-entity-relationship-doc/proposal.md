## Why

The `docs/` folder contains architecture and setup notes but lacks a dedicated entity-relationship reference. Developers need a single document that maps all domain entity associations to understand data flow and navigate the codebase efficiently.

## What Changes

- Add `docs/entity-relationships.md` — a comprehensive ER document covering all entities in `OrderSystem.Domain/Entities/`, including their fields, relationships, and cardinality

## Capabilities

### New Capabilities

- `entity-relationship-doc`: A markdown document in `docs/` that documents all entity relationships (ApplicationUser, Cart, CartItem, Order, OrderItem, Product, ProductImage) with fields, foreign keys, navigation properties, and a Mermaid ER diagram.

### Modified Capabilities

## Impact

- `docs/entity-relationships.md` — new file, no code changes required
