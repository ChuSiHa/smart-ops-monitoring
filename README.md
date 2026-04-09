# Smart Ops Monitoring

A full-stack **operations monitoring platform** that provides real-time visibility into the health and performance of hosts, services, and infrastructure. It collects and streams telemetry metrics, raises alerts on anomalies, and presents live dashboards with charts — all secured behind JWT authentication.

---

## Architecture

The system follows a layered **Clean Architecture** on the backend and a feature-based **Angular SPA** on the frontend, communicating via a REST API and real-time SignalR hubs.

```
┌──────────────────────────────────────────────────────────────────┐
│                        Browser (Angular 19)                      │
│  Dashboard │ Hosts │ Metrics │ Alerts │ Reports │ Settings       │
│            Angular Material + ECharts + SignalR client           │
└───────────────────────────┬──────────────────────────────────────┘
                            │  HTTP (REST) / WebSocket (SignalR)
┌───────────────────────────▼──────────────────────────────────────┐
│                    ASP.NET Core 8 API                            │
│  Controllers  │  SignalR Hubs  │  JWT Auth  │  Swagger UI        │
├──────────────────────────────────────────────────────────────────┤
│                    Application Layer                             │
│       CQRS (MediatR 12) │ FluentValidation 11 │ DTOs             │
├──────────────────────────────────────────────────────────────────┤
│                   Infrastructure Layer                           │
│  EF Core 8 + Npgsql │ ASP.NET Identity │ Hangfire │ Serilog      │
├──────────────────────────────────────────────────────────────────┤
│                      Domain Layer                                │
│        Entities │ Value Objects │ Domain Events │ Interfaces     │
└───────────────────────────┬──────────────────────────────────────┘
                            │
                ┌───────────▼───────────┐
                │      PostgreSQL        │
                │  (TimescaleDB ready)   │
                └───────────────────────┘
```

### Backend layer dependencies

```
Api → Infrastructure → Application → Domain
Api → Application
```

### Repository structure

```
smart-ops-monitoring/
├── src/
│   ├── SmartOpsMonitoring.Domain/          # Entities, enums, value objects, domain events, repository interfaces
│   ├── SmartOpsMonitoring.Application/     # CQRS commands/queries (MediatR), DTOs, validators, DI registration
│   ├── SmartOpsMonitoring.Infrastructure/  # EF Core, Identity, Hangfire jobs, SignalR hubs, Serilog sinks
│   └── SmartOpsMonitoring.Api/             # Controllers, middleware, JWT config, Swagger, Program.cs
└── smart-ops-ui/                           # Angular 19 SPA
    └── src/app/
        ├── core/                           # Auth guards, interceptors, services
        ├── features/                       # alerts │ auth │ dashboard │ hosts │ metrics │ reports │ settings
        └── shared/                         # Reusable components, pipes, models
```

---

## Tech Stack

### Backend

| Concern | Technology |
|---|---|
| Framework | ASP.NET Core 8 (Web API) |
| Architecture | Clean Architecture — Domain / Application / Infrastructure / Api |
| CQRS pipeline | MediatR 12 |
| Validation | FluentValidation 11 |
| ORM | Entity Framework Core 8 |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Real-time | ASP.NET Core SignalR (`MetricHub`, `AlertHub`) |
| Background jobs | Hangfire 1.8 (PostgreSQL storage) |
| Logging | Serilog 8 → Console + PostgreSQL sink |
| API docs | Swashbuckle / Swagger UI with Bearer token support |

### Frontend

| Concern | Technology |
|---|---|
| Framework | Angular 19 |
| UI components | Angular Material 19 |
| Charts | Apache ECharts 6 via ngx-echarts 19 |
| Real-time | @microsoft/signalr 10 |
| Reactive state | RxJS 7.8 |
| Language | TypeScript 5.6 |
| Styles | SCSS |

### Database

| Concern | Technology |
|---|---|
| Primary database | PostgreSQL |
| ORM driver | Npgsql (EF Core provider) |
| Time-series extension | TimescaleDB (optional) |
| Job storage | PostgreSQL (Hangfire schema) |
| Log storage | PostgreSQL (Serilog sink) |

---

## Domain Model

| Entity | Key fields |
|---|---|
| `Host` | Name, IpAddress, OsType, Status, Tags |
| `ServiceNode` | Name, Type, HostId, Status, Port |
| `Metric` | HostId, MetricType, Value, Unit, Timestamp, Labels |
| `Alert` | HostId, Title, Severity, Status, AcknowledgedAt, ResolvedAt |

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) and npm
- PostgreSQL (local install or Docker)

### 1. Configure secrets

`Jwt:Key` must be supplied via environment variable or .NET User Secrets — it is **not** committed to source:

```bash
# Development (User Secrets)
dotnet user-secrets set "Jwt:Key" "your-256-bit-secret" \
  --project src/SmartOpsMonitoring.Api

# Production (environment variable)
export Jwt__Key="your-256-bit-secret"
export ConnectionStrings__DefaultConnection="Host=...;Database=smartops;Username=...;Password=..."
```

### 2. Set the connection string

Update `src/SmartOpsMonitoring.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=smartops;Username=postgres;Password=postgres"
  }
}
```

### 3. Run the backend

```bash
# Apply migrations and start the API
dotnet run --project src/SmartOpsMonitoring.Api
```

| Endpoint | URL |
|---|---|
| Swagger UI | `https://localhost:{port}/swagger` |
| Hangfire Dashboard | `https://localhost:{port}/hangfire` |
| MetricHub (WebSocket) | `wss://localhost:{port}/hubs/metrics` |
| AlertHub (WebSocket) | `wss://localhost:{port}/hubs/alerts` |

### 4. Run the frontend

```bash
cd smart-ops-ui
npm install
npm start        # ng serve — proxies API calls to localhost:5000
```

The Angular dev server starts on `http://localhost:4200`.

---

## API Overview

| Controller | Base route | Auth | Description |
|---|---|---|---|
| Auth | `/api/auth` | Public | Register & login (returns JWT) |
| Hosts | `/api/hosts` | Required | Host CRUD |
| ServiceNodes | `/api/servicenodes` | Required | Service node management |
| Metrics | `/api/metrics` | Required | Ingest & query telemetry metrics |
| Alerts | `/api/alerts` | Required | Alert lifecycle management |

## Real-Time Hubs

| Hub | Path | Description |
|---|---|---|
| MetricHub | `/hubs/metrics` | Stream live metric data per host |
| AlertHub | `/hubs/alerts` | Stream live alert notifications |

Clients join named groups per host (`JoinHostGroup(hostId)`) so each client receives only targeted updates.

## Hangfire Recurring Jobs

| Job | Schedule |
|---|---|
| Metric aggregation | Every 5 minutes |
| Stale alert cleanup | Every hour |
| Health-check polling | Every minute |
