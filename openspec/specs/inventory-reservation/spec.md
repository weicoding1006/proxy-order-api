# inventory-reservation Specification

## Purpose
TBD - created by archiving change order-status-inventory-management. Update Purpose after archive.
## Requirements
### Requirement: Product tracks reserved stock separately from available stock
The system SHALL add a `ReservedStock` integer field (default 0) to `Product`. Available stock is defined as `Stock - ReservedStock`. The system SHALL use available stock (not raw `Stock`) for all "can we fulfill this?" checks.

#### Scenario: Available stock calculation
- **WHEN** a product has `Stock = 10` and `ReservedStock = 3`
- **THEN** available stock SHALL be `7`

#### Scenario: ReservedStock defaults to zero for existing products
- **WHEN** a product was created before this change
- **THEN** `ReservedStock` SHALL default to `0`, meaning all existing stock is available

---

### Requirement: Order creation reserves inventory
The system SHALL atomically increment `Product.ReservedStock` for each order item when a new order is created with status `Pending`. If available stock is insufficient, the order SHALL be rejected.

#### Scenario: Sufficient available stock
- **WHEN** a user creates an order for quantity Q of a product with available stock >= Q
- **THEN** the order SHALL be created with status `Pending` and `ReservedStock` SHALL increase by Q

#### Scenario: Insufficient available stock
- **WHEN** a user creates an order for quantity Q of a product with available stock < Q
- **THEN** the order SHALL be rejected with HTTP 400 and `ReservedStock` SHALL not change

#### Scenario: Concurrent orders do not oversell
- **WHEN** two requests simultaneously try to reserve the last unit of a product
- **THEN** exactly one SHALL succeed and the other SHALL receive a stock-insufficient error

---

### Requirement: Order confirmation deducts stock and releases reservation
The system SHALL, upon transition from `Pending` to `Confirmed`, atomically decrement `Product.Stock` by each item's quantity and decrement `Product.ReservedStock` by the same quantity.

#### Scenario: Stock is formally deducted on confirmation
- **WHEN** an Admin confirms a Pending order with quantity Q for a product
- **THEN** `Product.Stock` SHALL decrease by Q and `Product.ReservedStock` SHALL decrease by Q

---

### Requirement: Order cancellation returns reserved or deducted inventory
The system SHALL restore inventory when an order is cancelled, depending on its prior status:
- Cancelled from `Pending`: decrement `ReservedStock` by each item's quantity
- Cancelled from `Confirmed`: increment `Stock` by each item's quantity

#### Scenario: Cancel a Pending order returns reserved stock
- **WHEN** a Pending order with reserved quantity Q is cancelled
- **THEN** `Product.ReservedStock` SHALL decrease by Q and `Product.Stock` SHALL not change

#### Scenario: Cancel a Confirmed order returns deducted stock
- **WHEN** a Confirmed order with quantity Q is cancelled
- **THEN** `Product.Stock` SHALL increase by Q and `Product.ReservedStock` SHALL not change

