# Smart Ops Monitoring — Backend API

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
