# AGENTS.md

## Purpose
This file defines mandatory repository workflow and implementation rules for AI/code agents working in **RealTimeChatApp**.
Follow these rules strictly unless the user explicitly overrides them.

## Project Snapshot
- Project: `RealTimeChatApp`
- Stack: ASP.NET Core Web API, Entity Framework Core, SQL Server, Redis (`StackExchange.Redis`), JWT Auth, SSE
- Architecture: N-Layer (`Api`, `Business`, `DataAccess`, `Entities`, `Core`)

## Layer Boundaries (Mandatory)
- `Api`: Controllers only. No business logic.
- `Business`: Service abstractions + concrete service implementations.
- `DataAccess`: DAL abstractions + EF implementations + EF configurations + DbContexts.
- `Entities`: Entity classes, DTOs, enums.
- `Core`: Shared infrastructure (`BaseEntity`, `IEntity`, `IEntityRepository`, `EfEntityRepositoryBase`).

## Hard Rules
- Use `Guid` for all entity IDs. Never use `int` IDs for entities.
- Controllers must not implement service interfaces directly.
- `DbSet<>` properties in `DbContext` must be `public`.
- `EfEntityRepositoryBase` already handles standard CRUD.
- Keep DAL EF classes empty unless custom behavior is required.
- EF configurations are auto-loaded via `ApplyConfigurationsFromAssembly`.
- Add new `IEntityTypeConfiguration<>` classes normally; do not manually register each one.
- `RoomMember` uses composite key: `(RoomId, UserId)`.

## Redis Rules
- Redis must be running before operations that initialize app services used during startup-sensitive workflows.
- Redis configuration key: `Redis:ConnectionString`.
- Use `abortConnect=false` in Redis connection string.
- Chat cache key format: `chat:room:{roomId}:messages`.
- Pub/Sub channel format: `chatroom:{roomId}`.
- Cache latest 50 room messages with `ListRightPush` + `ListTrim`.

## EF Core + Migration Workflow
- Migration command must use:
  - `--project` => `DataAccess`
  - `--startup-project` => `Api`
- A design-time factory (`IDesignTimeDbContextFactory`) exists in `DataAccess`; preserve compatibility when editing context setup.
- Default DB connection string key: `ConnectionStrings:DefaultConnection` in `appsettings.json`.

### Canonical Commands
```powershell
# Add migration
 dotnet ef migrations add <MigrationName> --project DataAccess --startup-project Api

# Update database
 dotnet ef database update --project DataAccess --startup-project Api
```

## JWT Rules
- Required config keys:
  - `Jwt:Key`
  - `Jwt:Issuer`
  - `Jwt:Audience`
  - `Jwt:ExpireMinutes`
- Do not use `ExpirationTimeInMinutes`.
- Claims must be passed explicitly through the `claims:` parameter of `JwtSecurityToken`.
- If claims are omitted, generated tokens may be effectively empty.

## SSE Rules
- Required response headers:
  - `Content-Type: text/event-stream`
  - `Cache-Control: no-cache`
  - `Connection: keep-alive`
- Message payload format must be exactly:
  - `data: {payload}\n\n`
- The double newline is mandatory for proper event framing.

## Swagger/Auth Rules
- Swagger uses Bearer token auth scheme.
- Token input format must be: `Bearer {token}` (with a space).

## Implementation Guidance for Agents
- Keep each change in its correct layer; do not leak responsibilities across layers.
- Prefer minimal, targeted edits over broad refactors.
- Preserve naming and folder conventions already used in the repository.
- When adding entities or DTOs, ensure all references (Business/DataAccess/API) use `Guid` IDs consistently.
- When adding new endpoints, keep controller thin and delegate logic to Business services.
- When adding persistence behavior, first check whether `EfEntityRepositoryBase` already provides it.
- When debugging auth issues, verify config key names before changing code.
- When debugging SSE issues, verify headers and message framing before deeper changes.

## Pre-PR Checklist
- Build passes.
- Layer boundaries are respected.
- No entity uses `int` as ID.
- EF migrations use correct project/startup project pairing.
- JWT config keys and token claim mapping are correct.
- SSE headers and `data: ...\n\n` framing are correct.
- Redis key/channel naming matches required format.

## Notes for Future Agents
If there is any conflict between generic framework habits and this file, follow this file for this repository.
