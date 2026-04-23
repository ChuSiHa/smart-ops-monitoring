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

### Option A — Docker Compose (recommended)

Runs the entire stack (PostgreSQL × 2, Redis, Elasticsearch, Kibana, API) with a single command.

#### Prerequisites
- [Docker Desktop](https://docs.docker.com/get-docker/) ≥ 24 (includes Compose v2)

#### 1. Create your `.env` file

```bash
cp .env.example .env
```

Edit `.env` and replace every placeholder value, especially:

| Variable | Description |
|---|---|
| `POSTGRES_PASSWORD` | PostgreSQL password |
| `JWT_KEY` | Random string ≥ 32 characters — e.g. `openssl rand -base64 32` |
| `ELASTIC_PASSWORD` | `elastic` superuser password |
| `KIBANA_SYSTEM_PASSWORD` | `kibana_system` user password |
| `KIBANA_ENCRYPTION_KEY` | Random hex string ≥ 32 chars — e.g. `openssl rand -hex 32` |

#### 2. Raise the host `vm.max_map_count` (Linux only)

Elasticsearch requires a higher virtual-memory limit on Linux hosts:

```bash
sudo sysctl -w vm.max_map_count=262144
# Persist across reboots:
echo "vm.max_map_count=262144" | sudo tee -a /etc/sysctl.conf
```

On macOS and Windows (Docker Desktop) this is handled automatically inside the VM.

#### 3. Start the stack

```bash
docker compose up -d
```

Startup order is managed automatically:
1. PostgreSQL & Redis become healthy.
2. Elasticsearch becomes healthy.
3. `elasticsearch-setup` (one-shot) sets the `kibana_system` password.
4. Kibana starts and connects to Elasticsearch.
5. API starts and auto-migrates the database.

#### 4. Verify

| Service | URL | Credentials |
|---|---|---|
| API / Swagger | http://localhost:8080/swagger | — |
| Hangfire | http://localhost:8080/hangfire | app user (JWT) |
| Elasticsearch | http://localhost:9200 | `elastic` / `$ELASTIC_PASSWORD` |
| Kibana | http://localhost:5601 | `elastic` / `$ELASTIC_PASSWORD` |

Check container health:

```bash
docker compose ps
```

#### 5. Stop

```bash
docker compose down          # keep volumes
docker compose down -v       # remove volumes (destroys all data)
```

---

### Option B — Local development (without Docker)

#### Prerequisites
- .NET 8 SDK
- PostgreSQL (or Docker)

#### 1. Configure secrets

`Jwt:Key` must be supplied via environment variable or .NET User Secrets — it is **not** in source:

```bash
# Development
dotnet user-secrets set "Jwt:Key" "your-256-bit-secret" \
  --project src/SmartOpsMonitoring.Api

# Production — environment variable
export Jwt__Key="your-256-bit-secret"
```

#### 2. Set connection strings

Update `src/SmartOpsMonitoring.Api/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=smartops;Username=postgres;Password=postgres"
  }
}
```

#### 3. Run migrations

```bash
cd src/SmartOpsMonitoring.Api
dotnet ef database update --project ../SmartOpsMonitoring.Infrastructure
```

#### 4. Run the API

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

