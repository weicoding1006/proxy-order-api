## Context

The project already has several markdown files in `docs/` (architecture, secrets-setup, order-status-and-inventory, identity-dbcontext). An entity-relationship document is missing. This is a documentation-only change — no code is modified.

## Goals / Non-Goals

**Goals:**
- Document all 7 domain entities with their fields and types
- Show cardinality (1:1, 1:N) for every relationship
- Include a Mermaid ER diagram for visual navigation
- Note design decisions (e.g., UnitPrice snapshot, ReservedStock, RowVersion)

**Non-Goals:**
- Changing any entity code
- Documenting EF Core configuration (that belongs in identity-dbcontext.md)
- API-level documentation

## Decisions

**Use Mermaid `erDiagram`**: GitHub renders Mermaid natively, so the diagram is visible directly in the browser without extra tooling.

**Single file at `docs/entity-relationships.md`**: Keeps all relationship info in one place, consistent with the existing docs convention of one-file-per-topic.

## Risks / Trade-offs

- [Diagram drifts from code] → Low risk for a doc-only change; future entity changes must update this file alongside the entity class.
