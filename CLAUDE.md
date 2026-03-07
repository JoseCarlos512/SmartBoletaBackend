# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build SmartBoleta.sln

# Run API (from repo root)
dotnet run --project SmartBoleta.API

# EF Core migrations (run from repo root)
dotnet ef migrations add <MigrationName> --project SmartBoleta.Infrastructure --startup-project SmartBoleta.API
dotnet ef database update --project SmartBoleta.Infrastructure --startup-project SmartBoleta.API

# Tests (no test project yet)
dotnet test
```

API runs at `https://localhost:7xxx`. In Development: Swagger at `/swagger/index.html`, Hangfire dashboard at `/hangfire`.

## Architecture

**Clean Architecture + DDD** with four projects:

| Project | Role |
|---|---|
| `SmartBoleta.Domain` | Entities, `BaseEntity`, `Result`/`Error`, repository interfaces, service interfaces (`IStorageService`, `IOcrService`, `INotificationService`, `IJobScheduler`, `ISqlConnectionFactory`), security interfaces, `Roles` constants |
| `SmartBoleta.Infrastructure` | EF Core (`SmartBoletaDbContext`), write repositories, `SqlConnectionFactory`, JWT/password services, local storage, OCR stub, Hangfire jobs, SignalR hub + notification service |
| `SmartBoleta.Application` | MediatR commands/queries/handlers, DTOs, `ValidationBehavior` pipeline |
| `SmartBoleta.API` | Controllers, request models, middleware, `Program.cs` |

Dependency direction: `API` → `Application` → `Infrastructure` → `Domain`. All cross-cutting interfaces live in **Domain** so both Application and Infrastructure can reference them without circular dependencies.

## Key Patterns

**Result pattern** — all handlers return `Result<T>` or `Result`. Use `Result.Success(value)`, `Result.Failure<T>(error)`. Domain errors are `static readonly Error` fields on `XxxErrors` classes co-located with the entity (e.g. `BoletasErrors.NotFound`).

**CQRS via MediatR** — commands implement `ICommand<TResponse>` or `ICommand`. Queries implement `IQuery<TResponse>`. Handler and validator are `internal sealed` classes in the **same file** as their command/query record.

**FluentValidation pipeline** — `ValidationBehavior<TRequest, TResponse>` runs automatically on every `IBaseCommand` before the handler. On failure it throws `FluentValidation.ValidationException`, which `ExceptionHandlingMiddleware` catches and returns as HTTP 400 with a structured `{ errors }` body. Every command file must include a validator.

**Module structure in Application:**
```
Application/
  Behaviors/ValidationBehavior.cs
  Modules/
    Boletas/
      Command/SubirBoletaCommand.cs    # record + handler + validator (internal sealed)
      Command/FirmarBoletaCommand.cs
      Query/ObtenerBoletaQuery.cs      # Dapper, projects to BoletaDto
      Query/ObtenerBoletasPorUsuarioQuery.cs
      Query/ObtenerBoletasPorTenantQuery.cs   # paginated (Pagina, TamanoPagina)
      DTOs/BoletaDto.cs
    Tenants/...
    Usuarios/...
    Auths/...
  Abstractions/Messaging/              # ICommand, IQuery, ICommandHandler, IQueryHandler
```

**Hybrid data access (EF Core + Dapper)**
- **Commands (writes)** → repository interfaces → EF Core. Interfaces contain only `AddAsync`/`UpdateAsync` plus reads required by command logic (`ObtenerUsuarioPorCorreo` for login, `ObtenerPorId` for Firmar).
- **Queries (reads)** → handlers inject `ISqlConnectionFactory`, open a `using var connection`, run explicit SQL with column aliases, map directly to DTOs. No entity hydration.

Column alias rules for Dapper SQL: `BoletaId AS Id`, `TenantId AS Id` (Tenants), `UsuarioId AS Id`, `RUC AS Ruc`. `BoletaEstado` maps as `int` — Dapper converts automatically to the enum.

**Database** — SQL Server, EF Core. `UseSnakeCaseNamingConvention()` is called but `OnModelCreating` overrides all column names explicitly (PascalCase). PKs use `NEWSEQUENTIALID()`. `bool` → `TINYINT`, `BoletaEstado` enum → `int`.

**Authentication** — JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`). Config key `"Jwt"` (Issuer, Audience, Secret, TokenLifetimeMinutes). Token claims: `sub` (userId), `tenantId`, `email`, `ClaimTypes.Role`. Controllers extract `tenantId`/`sub` from `User.FindFirst(...)` — never from headers.

**Authorization** — role-based via `[Authorize(Roles = Roles.Admin)]`. Role constants in `SmartBoleta.Domain.Roles` (Admin, Manager, User). Stored on `Usuario.Rol` (NVARCHAR(20), default "User").

**Enums** serialize as strings in JSON responses via `JsonStringEnumConverter` registered globally in `Program.cs`.

**SignalR** — `NotificacionHub` at `/hubs/notificaciones`. Clients call `UnirseATenant(tenantId)` to join a group. JWT passed via `?access_token=` query string for WebSocket connections. `INotificationService` (`SignalRNotificationService`) sends by tenant group or by user.

**Background jobs (Hangfire)** — SQL Server storage, same connection string as EF Core. `IJobScheduler.EnqueueOcrJob(boletaId)` enqueues `OcrBackgroundJob.ProcesarAsync`. Job flow: update estado → `ProcesandoOcr` → call `IOcrService.ExtraerTextoAsync` → `ActualizarOcr` → `Disponible` → notify via SignalR.

**Storage** — `IStorageService` / `LocalStorageService` saves files to `Storage:LocalPath` config path (default `./storage/`). Returns a unique filename as the URL identifier.

**OCR** — `LocalOcrService` is a stub returning `string.Empty`. Replace with Tesseract (`Tesseract` NuGet) or Azure Cognitive Services (`Azure.AI.Vision.ImageAnalysis`).

**DI registration** — `AddApplication()` wires MediatR + `ValidationBehavior` + validators. `AddInfrastructure(config)` wires EF Core, repositories, security, storage, OCR, SignalR, Hangfire, and `SqlConnectionFactory`.

## Domain Entities

| Entity | Key fields | Notes |
|---|---|---|
| `Tenant` | NombreComercial, Ruc, LogoUrl, ColorPrimario, FaviconUrl, Estado | Multi-tenant company |
| `Usuario` | TenantId, Nombre, Correo, DNI, Rol, PasswordHash, PasswordSalt, Estado | PBKDF2 password; Rol defaults to "User" |
| `Boleta` | TenantId, UsuarioId, Periodo (YYYY-MM), ArchivoNombre, ArchivoUrl, Estado, TextoOcr, FechaSubida, FechaFirma | Created via `Boleta.Create(...)`, signed via `boleta.Firmar()` which enforces `Estado == Disponible` |

## Conventions

- Spanish naming for domain concepts (entity properties, command fields, error messages, method names).
- Errors: `static readonly Error` fields, format `"EntityName.ErrorType"` (e.g. `"Boletas.AccesoDenegado"`).
- Each command file contains: `record Command`, `internal sealed class Handler`, `internal sealed class Validator`.
- New service interfaces go in `SmartBoleta.Domain/Abstractions/`; implementations go in `SmartBoleta.Infrastructure/Services/`.
- `BoletaEstado` Dapper queries do **not** need `CAST` — int-to-enum mapping is automatic. `Estado` on Tenant/Usuario is `TINYINT`/`bool` and **does** need `CAST(Estado AS BIT) AS Estado`.
