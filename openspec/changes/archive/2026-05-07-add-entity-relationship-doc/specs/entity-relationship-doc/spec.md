## ADDED Requirements

### Requirement: Entity relationship document exists in docs/
The project SHALL have a `docs/entity-relationships.md` file that documents all domain entities, their fields, and their associations.

#### Scenario: File is present
- **WHEN** a developer opens the `docs/` directory
- **THEN** `entity-relationships.md` is present alongside the other doc files

### Requirement: All 7 entities are documented
The document SHALL cover ApplicationUser, Cart, CartItem, Order, OrderItem, Product, and ProductImage — each with a field table listing name, type, and purpose.

#### Scenario: Entity field table
- **WHEN** a developer reads the document
- **THEN** each entity section contains a markdown table with columns: Field, Type, Description

### Requirement: Relationships are described with cardinality
The document SHALL describe every association (foreign key / navigation property) with its cardinality (1:1 or 1:N).

#### Scenario: Relationship list
- **WHEN** a developer reads the Relationships section
- **THEN** each relationship entry states the two entities, the direction, and the cardinality

### Requirement: Mermaid ER diagram is included
The document SHALL include a `mermaid` code block using `erDiagram` syntax so the diagram renders on GitHub.

#### Scenario: Diagram renders
- **WHEN** the file is viewed on GitHub
- **THEN** the Mermaid block renders as a visual ER diagram showing all entities and their connections

### Requirement: Design decisions are noted
The document SHALL include a section explaining non-obvious design choices: UnitPrice price snapshot on OrderItem, ReservedStock on Product, and RowVersion for optimistic concurrency.

#### Scenario: Design note present
- **WHEN** a developer reads the Design Notes section
- **THEN** UnitPrice, ReservedStock, and RowVersion are each explained in one or two sentences
