# Smart Ops Monitoring — Backend API

**.NET 8 Clean Architecture** backend for the Smart Operations Monitoring platform.

## Architecture

```
src/
├── SmartOpsMonitoring.Domain/          # Entities, value objects, domain events, repository interfaces
├── SmartOpsMonitoring.Application/     # CQRS (MediatR), DTOs, FluentValidation, use-case handlers
├── SmartOpsMonitoring.Infrastructure/  # EF Core + Npgsql, Identity, Hangfire, SignalR hubs, Serilog
└── SmartOpsMonitoring.Api/             # Controllers, hubs routes, JWT auth, Swagger, middleware
```

### Layer dependencies
```
Api → Infrastructure → Application → Domain
Api → Application
```

## Tech stack

| Concern | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| ORM | Entity Framework Core 8 + Npgsql (PostgreSQL) |
| CQRS | MediatR 12 |
| Validation | FluentValidation 11 |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Real-time | ASP.NET Core SignalR (`MetricHub`, `AlertHub`) |
| Scheduler | Hangfire 1.8 with PostgreSQL storage |
| Logging | Serilog → Console + PostgreSQL |
| API Docs | Swashbuckle / Swagger UI with ****** |
| DB | PostgreSQL (TimescaleDB extension optional) |

## Getting started

### Prerequisites
- .NET 8 SDK
- PostgreSQL (or Docker)

### 1. Configure secrets

`Jwt:Key` must be supplied via environment variable or .NET User Secrets — it is **not** in source:

```bash
# Development
dotnet user-secrets set "Jwt:Key" "your-256-bit-secret" \
  --project src/SmartOpsMonitoring.Api

# Production — environment variable
export Jwt__Key="your-256-bit-secret"
```

### 2. Set connection strings

Update `src/SmartOpsMonitoring.Api/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=smartops;Username=postgres;Password=postgres"
  }
}
```

### 3. Run migrations

```bash
cd src/SmartOpsMonitoring.Api
dotnet ef database update --project ../SmartOpsMonitoring.Infrastructure
```

### 4. Run the API

```bash
dotnet run --project src/SmartOpsMonitoring.Api
```

- Swagger UI: `https://localhost:{port}/swagger`
- Hangfire Dashboard: `https://localhost:{port}/hangfire` (requires auth)
- MetricHub: `wss://localhost:{port}/hubs/metrics`
- AlertHub: `wss://localhost:{port}/hubs/alerts`

## API overview

| Controller | Base route | Min role |
|---|---|---|
| Auth | `/api/auth` | Public |
| Hosts | `/api/hosts` | Authenticated |
| ServiceNodes | `/api/servicenodes` | Authenticated |
| Metrics | `/api/metrics` | Authenticated |
| Alerts | `/api/alerts` | Authenticated |

## Domain model

| Entity | Key fields |
|---|---|
| `Host` | Name, IpAddress, OsType, Status, Tags |
| `ServiceNode` | Name, Type, HostId, Status, Port |
| `Metric` | HostId, MetricType, Value, Unit, Timestamp, Labels |
| `Alert` | HostId, Title, Severity, Status, AcknowledgedAt, ResolvedAt |

## SignalR groups

Clients join named groups per host: `JoinHostGroup(hostId)`. The infrastructure pushes metric and alert events to the appropriate group so each client receives only targeted updates.

## Hangfire recurring jobs

| Job | Schedule |
|---|---|
| Metric aggregation | Every 5 minutes |
| Stale alert cleanup | Every hour |
| Health-check polling | Every minute |


ASP.NET Core 8 Clean Architecture Web API for the Smart Operations Monitoring platform.

## Tech stack

- **.NET 8** / ASP.NET Core Web API (Clean Architecture — Domain, Application, Infrastructure, Api)
- **Entity Framework Core 8** + **PostgreSQL** (via Npgsql)
- **MediatR 12** — CQRS command/query pipeline
- **FluentValidation 11** — request validation
- **Hangfire 1.8** — background/recurring jobs (PostgreSQL storage)
- **SignalR** — real-time metric & alert streaming
- **Serilog 8** — structured logging
- **JWT Bearer** authentication with ASP.NET Core Identity
- **Swashbuckle / Swagger UI** with Bearer token support

## Getting started

### 1. Configure secrets

**Never commit real secrets.** The `appsettings.json` file ships with placeholder values. Override them before running in any non-local environment:

```bash
# Via environment variable (recommended)
export Jwt__Key="your-256-bit-secret-key-here"
export ConnectionStrings__DefaultConnection="Host=...;Database=smartops;Username=...;Password=..."

# Or via .NET User Secrets (development only)
dotnet user-secrets set "Jwt:Key" "your-256-bit-secret-key-here" \
  --project src/SmartOpsMonitoring.Api
```

For production, use a secrets manager (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault) and inject via environment variables or a configuration provider.

### 2. Run the API

```bash
cd src/SmartOpsMonitoring.Api
dotnet run
```

The PostgreSQL database is migrated automatically on first startup.

Swagger UI is available at: `https://localhost:{port}/swagger`

## API overview

| Controller     | Base route               | Auth required | Description                        |
|----------------|--------------------------|---------------|------------------------------------|
| Auth           | `/api/auth`              | Public        | Register & login (returns JWT)     |
| Hosts          | `/api/hosts`             | Authenticated | Host CRUD                          |
| ServiceNodes   | `/api/servicenodes`      | Authenticated | Service node management            |
| Metrics        | `/api/metrics`           | Authenticated | Ingest & query telemetry metrics   |
| Alerts         | `/api/alerts`            | Authenticated | Alert lifecycle management         |

## Real-time hubs

| Hub        | Path               | Description                        |
|------------|--------------------|------------------------------------|
| MetricHub  | `/hubs/metrics`    | Stream live metric data per host   |
| AlertHub   | `/hubs/alerts`     | Stream live alert notifications    |

## Project structure

```
src/
├── SmartOpsMonitoring.Domain/          # Entities, enums, value objects, events, repository interfaces
├── SmartOpsMonitoring.Application/     # CQRS commands/queries, DTOs, validators, DI registration
├── SmartOpsMonitoring.Infrastructure/  # EF Core, Identity, repositories, hubs, Hangfire jobs
└── SmartOpsMonitoring.Api/             # Controllers, middleware, Program.cs
```
