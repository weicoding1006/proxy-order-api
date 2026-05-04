# order-status-transition Specification

## Purpose
TBD - created by archiving change order-status-inventory-management. Update Purpose after archive.
## Requirements
### Requirement: Order status is represented as a typed enum
The system SHALL define `OrderStatus` as an enum with values: `Pending`, `Confirmed`, `Shipped`, `Completed`, `Cancelled`. The domain entity `Order.Status` SHALL use this enum type. EF Core SHALL persist it as a string column to preserve existing data.

#### Scenario: New order defaults to Pending
- **WHEN** a new order is created
- **THEN** `Order.Status` SHALL equal `OrderStatus.Pending`

#### Scenario: JSON serialization preserves string format
- **WHEN** an order response is serialized to JSON
- **THEN** the `status` field SHALL be a string (e.g., `"Pending"`) not a numeric value

---

### Requirement: Order status transitions are enforced by the domain
The system SHALL only allow the following state transitions:

| From | To | Allowed By |
|------|----|-----------|
| Pending | Confirmed | Admin |
| Pending | Cancelled | Admin or Order Owner |
| Confirmed | Shipped | Admin |
| Confirmed | Cancelled | Admin |
| Shipped | Completed | Admin |

Any other transition SHALL be rejected.

#### Scenario: Admin confirms a pending order
- **WHEN** an Admin requests transition from `Pending` to `Confirmed`
- **THEN** the order status SHALL be updated to `Confirmed`

#### Scenario: Order owner cancels their own pending order
- **WHEN** the order owner requests transition from `Pending` to `Cancelled`
- **THEN** the order status SHALL be updated to `Cancelled`

#### Scenario: Invalid transition is rejected
- **WHEN** any caller requests a transition not listed in the allowed table (e.g., `Pending` → `Shipped`)
- **THEN** the system SHALL return HTTP 422 with an error message describing the invalid transition

#### Scenario: Non-admin attempts Admin-only transition
- **WHEN** a non-Admin user requests a transition reserved for Admins (e.g., `Confirmed` → `Shipped`)
- **THEN** the system SHALL return HTTP 403 Forbidden

---

### Requirement: Admin can update order status via API
The system SHALL expose `PATCH /api/orders/{id}/status` for updating an order's status.

#### Scenario: Admin updates order status successfully
- **WHEN** an Admin sends `PATCH /api/orders/{id}/status` with a valid target status
- **THEN** the system SHALL apply the transition and return the updated order (HTTP 200)

#### Scenario: Order not found
- **WHEN** the requested order ID does not exist
- **THEN** the system SHALL return HTTP 404

#### Scenario: Unauthenticated request
- **WHEN** the request has no valid JWT
- **THEN** the system SHALL return HTTP 401

