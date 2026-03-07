# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build SmartBoleta.sln

# Run API (from repo root)
dotnet run --project SmartBoleta.API

# EF Core migrations (run from repo root; startup project is API, migrations project is Infrastructure)
dotnet ef migrations add <MigrationName> --project SmartBoleta.Infrastructure --startup-project SmartBoleta.API
dotnet ef database update --project SmartBoleta.Infrastructure --startup-project SmartBoleta.API

# Tests (no test project yet)
dotnet test
```

The API runs at `https://localhost:7xxx` with Swagger at `/swagger/index.html` in Development.

## Architecture

**Clean Architecture + DDD** with four projects:

| Project | Role |
|---|---|
| `SmartBoleta.Domain` | Entities, `BaseEntity`, `Result`/`Error`, repository interfaces, security interfaces |
| `SmartBoleta.Infrastructure` | EF Core (`SmartBoletaDbContext`), repositories, `JwtTokenService`, `Pbkdf2PasswordHasher`, migrations |
| `SmartBoleta.Application` | MediatR commands/queries, handlers, DTOs |
| `SmartBoleta.API` | Controllers, request models, `Program.cs` |

Dependency direction: `API` → `Application` → `Infrastructure` → `Domain`. Note: `Application` currently also references `Infrastructure` directly.

## Key Patterns

**Result pattern** — all handlers return `Result<T>` or `Result`. Use `Result.Success(value)`, `Result.Failure<T>(error)`. Domain errors are defined as static fields on `XxxErrors` classes co-located with the entity (e.g. `TenantsErrors.NotFound`).

**CQRS via MediatR** — commands implement `ICommand<TResponse>` (returns `Result<TResponse>`) or `ICommand` (returns `Result`). Queries implement `IQuery<TResponse>`. Handlers are `internal sealed` classes in the same file as their command/query.

**Module structure in Application:**
```
Application/
  Modules/
    Tenants/
      Command/CrearTenantCommand.cs   # command record + internal handler
      Query/ObtenerTenantQuery.cs
      DTOs/TenantDto.cs
    Usuarios/...
    Auths/...
  Abstractions/Messaging/             # ICommand, IQuery, ICommandHandler, IQueryHandler
```

**Controllers** inject `ISender` (MediatR), map HTTP requests to commands/queries, and return `Ok`/`BadRequest`/`NotFound` based on `resultado.IsSuccess`.

**Database** — SQL Server with EF Core. `UseSnakeCaseNamingConvention()` is called but entity configurations in `OnModelCreating` override column names explicitly. PKs use `NEWSEQUENTIALID()`. Boolean fields mapped to `TINYINT`.

**JWT config** (`appsettings.json` → `"Jwt"` section): `Issuer`, `Audience`, `Secret`, `TokenLifetimeMinutes`. Bound via `services.Configure<JwtOptions>(...)`. Connection string key: `"SqlServerDatabase"`.

**DI registration** — each layer exposes an extension method: `AddApplication()` and `AddInfrastructure(configuration)`, both called in `Program.cs`.

## Current Domain Entities

- `Tenant` — multi-tenant company (NombreComercial, Ruc, LogoUrl, ColorPrimario, FaviconUrl)
- `Usuario` — belongs to a `Tenant` via `TenantId` FK; stores `PasswordHash`/`PasswordSalt` (PBKDF2), `Correo`, `DNI`

## Conventions

- Spanish naming for domain concepts (entity properties, command fields, error messages).
- Errors follow the format `"EntityName.ErrorType"` (e.g. `"Tenants.NotFound"`).
- `FluentValidation` is registered but validators have not been implemented yet.
