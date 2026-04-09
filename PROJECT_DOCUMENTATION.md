# SmartOps Monitoring — Project Documentation

## Table of Contents

1. [Functional Requirements](#1-functional-requirements)
2. [Non-Functional Requirements](#2-non-functional-requirements)
3. [Technology Stack](#3-technology-stack)
4. [Domain Entities](#4-domain-entities)
5. [Use Cases](#5-use-cases)
6. [Sequence Diagrams](#6-sequence-diagrams)
7. [Backend Setup](#7-backend-setup)
8. [Frontend Setup](#8-frontend-setup)
9. [Test Data Migration Script](#9-test-data-migration-script)

---

## 1. Functional Requirements

### 1.1 Authentication & Authorization

| ID | Requirement |
|----|-------------|
| FR-01 | The system shall allow any visitor to register a new account using an email address and password. |
| FR-02 | Registered users shall be able to log in and receive a short-lived JWT bearer token. |
| FR-03 | All API endpoints except `/api/auth/register` and `/api/auth/login` shall require a valid JWT token. |
| FR-04 | The system shall support two roles: **Admin** and **Operator**. |
| FR-05 | The **Settings** page shall be accessible only to users with the **Admin** role. |
| FR-06 | All other authenticated pages shall be accessible to both **Admin** and **Operator** users. |

### 1.2 Host Management

| ID | Requirement |
|----|-------------|
| FR-10 | Authenticated users shall be able to register a new monitored host, providing a name, IP address, OS type, and optional tags. |
| FR-11 | The system shall enforce unique host names. |
| FR-12 | Authenticated users shall be able to list all registered hosts. |
| FR-13 | Authenticated users shall be able to view the details of a single host by its ID, including its service nodes. |
| FR-14 | A host shall carry a status: `Unknown`, `Online`, `Offline`, or `Maintenance`. |

### 1.3 Service Node Management

| ID | Requirement |
|----|-------------|
| FR-20 | Authenticated users shall be able to register a service node (e.g., web server, database) on an existing host. |
| FR-21 | Each service node shall store a name, type, port, and status (`Unknown`, `Running`, `Stopped`, `Error`). |
| FR-22 | Authenticated users shall be able to list all service nodes belonging to a given host. |

### 1.4 Metric Ingestion & Querying

| ID | Requirement |
|----|-------------|
| FR-30 | Authenticated clients (agents/services) shall be able to ingest telemetry metric data points, including host ID, metric type, numeric value, unit, optional service node ID, and key-value labels. |
| FR-31 | Upon ingestion, the system shall publish a `MetricReceivedEvent` domain event. |
| FR-32 | The system shall push the ingested metric in real time to all connected dashboard clients subscribed to the relevant host group via SignalR. |
| FR-33 | Authenticated users shall be able to query metrics for a given host, optionally filtering by metric type and time range. |
| FR-34 | The system shall provide the latest metric per type for a given host. |
| FR-35 | A background job shall aggregate metrics every minute. |

### 1.5 Alert Management

| ID | Requirement |
|----|-------------|
| FR-40 | Authenticated users (and automated jobs) shall be able to create monitoring alerts with a title, message, severity (`Info`, `Warning`, `Critical`), host ID, and optional service node ID. |
| FR-41 | The system shall publish an `AlertCreatedEvent` domain event upon alert creation. |
| FR-42 | The system shall push new alerts in real time to subscribed dashboard clients via SignalR. |
| FR-43 | Authenticated users shall be able to list alerts filtered by host, status, and/or severity. |
| FR-44 | Authenticated users shall be able to update an alert's status to `Acknowledged` or `Resolved`. |
| FR-45 | When an alert is acknowledged, the system shall record the acknowledging user's ID and the acknowledgement timestamp. |
| FR-46 | When an alert is resolved, the system shall record the resolution timestamp. |
| FR-47 | A background job shall clean up stale open alerts daily. |

### 1.6 Health-Check Polling

| ID | Requirement |
|----|-------------|
| FR-50 | A background job shall poll registered hosts and service nodes for health status every 5 minutes. |

### 1.7 Dashboard & Reporting

| ID | Requirement |
|----|-------------|
| FR-60 | The frontend shall provide a dashboard overview displaying registered hosts, recent metrics, and open alerts. |
| FR-61 | The frontend shall provide a dedicated metrics view with time-series charts (Apache ECharts). |
| FR-62 | The frontend shall provide a dedicated alerts view for viewing and updating alert status. |
| FR-63 | The frontend shall provide a reports view for read-only summaries. |
| FR-64 | The frontend shall provide a settings view (Admin only). |

---

## 2. Non-Functional Requirements

### 2.1 Performance

| ID | Requirement |
|----|-------------|
| NFR-01 | The API shall respond to 95% of read requests within **500 ms** under normal load. |
| NFR-02 | Metric ingestion endpoints shall support at least **100 requests/second** on commodity hardware. |
| NFR-03 | The database shall use composite indices on `(HostId, Timestamp)` for metrics and `(Status, Severity)` for alerts to keep time-series queries sub-second. |
| NFR-04 | Real-time SignalR pushes shall reach connected clients within **1 second** of an event being raised. |

### 2.2 Scalability

| ID | Requirement |
|----|-------------|
| NFR-10 | The backend shall be stateless (JWT auth, SignalR groups) so that multiple API instances can run behind a load balancer. |
| NFR-11 | Hangfire jobs shall use PostgreSQL as durable storage, ensuring no job loss on restart. |
| NFR-12 | The database schema shall be compatible with a TimescaleDB hypertable upgrade for the Metrics table if scale demands it. |

### 2.3 Security

| ID | Requirement |
|----|-------------|
| NFR-20 | All API calls except auth endpoints shall require a valid, non-expired JWT token signed with HMAC-SHA256. |
| NFR-21 | JWT signing keys and database credentials shall never be committed to source control; they shall be supplied via environment variables or a secrets manager. |
| NFR-22 | User passwords shall be hashed by ASP.NET Core Identity (BCrypt-equivalent PBKDF2). |
| NFR-23 | The Hangfire dashboard shall require authentication before display. |
| NFR-24 | The application shall enforce password complexity (minimum 8 characters, at least one digit). |
| NFR-25 | Cross-origin requests shall be controlled via CORS policy (configurable per environment). |

### 2.4 Reliability & Availability

| ID | Requirement |
|----|-------------|
| NFR-30 | Database migrations shall run automatically on application startup to ensure zero-downtime schema updates. |
| NFR-31 | Background jobs shall be idempotent and retry-safe, managed by Hangfire. |
| NFR-32 | The application shall log all requests and unhandled exceptions via Serilog structured logging. |

### 2.5 Maintainability

| ID | Requirement |
|----|-------------|
| NFR-40 | The codebase shall follow Clean Architecture: Domain → Application → Infrastructure → API layers with strict dependency rules. |
| NFR-41 | All business logic shall be expressed as CQRS commands and queries handled by MediatR. |
| NFR-42 | Input validation shall be centralised in FluentValidation validators. |
| NFR-43 | The API shall expose a Swagger/OpenAPI v3 specification with Bearer token security scheme. |

### 2.6 Usability

| ID | Requirement |
|----|-------------|
| NFR-50 | The frontend shall use Angular Material for a consistent, responsive UI. |
| NFR-51 | Route-level lazy loading shall be used in the Angular application to minimise initial bundle size. |
| NFR-52 | The frontend shall intercept HTTP responses and handle 401 errors by redirecting to the login page. |

---

## 3. Technology Stack

### Backend

| Concern | Technology | Version |
|---------|-----------|---------|
| Runtime | .NET / ASP.NET Core | 8.0 |
| Architecture | Clean Architecture (Domain / Application / Infrastructure / Api) | — |
| ORM | Entity Framework Core + Npgsql | 8.0 |
| Database | PostgreSQL | 14+ (TimescaleDB optional) |
| CQRS Bus | MediatR | 12.4 |
| Object Mapping | Mapster | 7.4 |
| Validation | FluentValidation | 11.11 |
| Authentication | ASP.NET Core Identity + JWT Bearer | 8.0 |
| Real-time | ASP.NET Core SignalR | 8.0 |
| Background Jobs | Hangfire 1.8 (PostgreSQL storage) | 1.8 |
| Logging | Serilog (Console + PostgreSQL sinks) | 8.0 |
| API Documentation | Swashbuckle / Swagger UI | 6.9 |

### Frontend

| Concern | Technology | Version |
|---------|-----------|---------|
| Framework | Angular | 19.x |
| UI Components | Angular Material + CDK | 19.x |
| Charts | Apache ECharts (via ngx-echarts) | 6.x |
| Real-time | @microsoft/signalr | 10.x |
| HTTP Client | Angular HttpClient (with JWT interceptor) | 19.x |
| Reactive | RxJS | 7.8 |
| Language | TypeScript | 5.6 |
| Build Tool | Angular CLI / Vite | 19.x |

---

## 4. Domain Entities

### 4.1 Entity Relationship Diagram

```
┌────────────────────────────────┐
│           BaseEntity           │
│  + Id: Guid                    │
│  + CreatedAt: DateTime (UTC)   │
│  + UpdatedAt: DateTime (UTC)   │
└────────────────────────────────┘
          ▲       ▲       ▲       ▲
          │       │       │       │
   ┌──────┘  ┌───┘   ┌───┘   ┌───┘
   │         │       │       │
┌──┴──────────────┐  │  ┌────┴──────────────┐  ┌────────────────────┐
│      Host       │  │  │    ServiceNode    │  │       Alert        │
├─────────────────┤  │  ├───────────────────┤  ├────────────────────┤
│ Name            │  │  │ Name              │  │ HostId (FK)        │
│ IpAddress       │  │  │ Type              │  │ ServiceNodeId (FK) │
│ OsType          │  │  │ HostId (FK)       │  │ Title              │
│ Status          │  │  │ Status            │  │ Message            │
│ Tags            │  │  │ Port?             │  │ Severity           │
│ ServiceNodes ──►│  │  └───────────────────┘  │ Status             │
└─────────────────┘  │                         │ AcknowledgedAt?    │
                     │                         │ ResolvedAt?        │
         ┌───────────┘                         │ AcknowledgedByUser?│
         │                                     └────────────────────┘
┌────────┴──────────┐
│      Metric       │
├───────────────────┤
│ HostId (FK)       │
│ ServiceNodeId (FK)│
│ MetricType        │
│ Value             │
│ Unit              │
│ Timestamp         │
│ Labels (JSON)     │
└───────────────────┘
```

### 4.2 Entity Descriptions

#### Host
Represents a physical or virtual machine being monitored.

| Field | Type | Description |
|-------|------|-------------|
| Id | Guid | Primary key |
| Name | string (max 200) | Unique display name |
| IpAddress | string (max 45) | IPv4 or IPv6 address |
| OsType | string (max 100) | e.g. `Linux`, `Windows` |
| Status | HostStatus | `Unknown \| Online \| Offline \| Maintenance` |
| Tags | string[] | Comma-separated in DB |
| ServiceNodes | ServiceNode[] | Navigation property |

#### ServiceNode
Represents a service or process running on a host.

| Field | Type | Description |
|-------|------|-------------|
| Id | Guid | Primary key |
| Name | string (max 200) | Node display name |
| Type | string (max 100) | e.g. `web`, `database`, `queue` |
| HostId | Guid (FK) | Parent host |
| Status | ServiceNodeStatus | `Unknown \| Running \| Stopped \| Error` |
| Port | int? | Listening port |

#### Metric
A single telemetry data point collected from a host or service node.

| Field | Type | Description |
|-------|------|-------------|
| Id | Guid | Primary key |
| HostId | Guid (FK) | Source host |
| ServiceNodeId | Guid? (FK) | Optional source service node |
| MetricType | string (max 100) | e.g. `cpu_usage`, `memory_bytes` |
| Value | double | Numeric measurement |
| Unit | string (max 50) | e.g. `percent`, `bytes` |
| Timestamp | DateTime (UTC) | When measured |
| Labels | Dictionary\<string,string\> | Stored as JSON |

#### Alert
A monitoring alert raised against a host or service node.

| Field | Type | Description |
|-------|------|-------------|
| Id | Guid | Primary key |
| HostId | Guid (FK) | Target host |
| ServiceNodeId | Guid? (FK) | Optional target service node |
| Title | string (max 200) | Short description |
| Message | string (max 2000) | Detailed message |
| Severity | AlertSeverity | `Info \| Warning \| Critical` |
| Status | AlertStatus | `Open \| Acknowledged \| Resolved` |
| AcknowledgedAt | DateTime? | UTC time of acknowledgement |
| ResolvedAt | DateTime? | UTC time of resolution |
| AcknowledgedByUserId | string? | Identity user ID |

#### ApplicationUser (Identity)
Extends ASP.NET Core `IdentityUser`.

| Field | Type | Description |
|-------|------|-------------|
| Id | string | ASP.NET Identity primary key |
| Email | string | Login identifier |
| DisplayName | string | Display name |
| Roles | string[] | `Admin` or `Operator` |

### 4.3 Enumerations

```
HostStatus:        Unknown=0, Online=1, Offline=2, Maintenance=3
ServiceNodeStatus: Unknown=0, Running=1, Stopped=2, Error=3
AlertSeverity:     Info=0, Warning=1, Critical=2
AlertStatus:       Open=0, Acknowledged=1, Resolved=2
```

---

## 5. Use Cases

### 5.1 Use Case Diagram (text)

```
                          ┌──────────────────────────────────────────┐
                          │           SmartOps Monitoring             │
                          │                                            │
  ┌──────────┐            │  ┌──────────────────────────────────┐     │
  │  Visitor │────────────┼─►│  UC-01 Register Account          │     │
  └──────────┘            │  └──────────────────────────────────┘     │
                          │  ┌──────────────────────────────────┐     │
                          │  │  UC-02 Login                     │     │
                          │  └──────────────────────────────────┘     │
                          │                                            │
  ┌──────────┐            │  ┌──────────────────────────────────┐     │
  │ Operator │────────────┼─►│  UC-03 View Dashboard            │     │
  └────┬─────┘            │  ├──────────────────────────────────┤     │
       │                  │  │  UC-04 Register Host             │     │
       │                  │  ├──────────────────────────────────┤     │
       │                  │  │  UC-05 View Host Details         │     │
       │                  │  ├──────────────────────────────────┤     │
       │                  │  │  UC-06 Add Service Node          │     │
       │                  │  ├──────────────────────────────────┤     │
       │                  │  │  UC-07 Ingest Metric             │     │
       │                  │  ├──────────────────────────────────┤     │
       │                  │  │  UC-08 Query Metrics             │     │
       │                  │  ├──────────────────────────────────┤     │
       │                  │  │  UC-09 Create Alert              │     │
       │                  │  ├──────────────────────────────────┤     │
       │                  │  │  UC-10 Acknowledge / Resolve     │     │
       │                  │  │        Alert                     │     │
       │                  │  ├──────────────────────────────────┤     │
       │                  │  │  UC-11 View Reports              │     │
       │                  │  └──────────────────────────────────┘     │
       │                  │                                            │
  ┌────┴─────┐            │  ┌──────────────────────────────────┐     │
  │  Admin   │────────────┼─►│  UC-12 Manage Settings           │     │
  └──────────┘            │  └──────────────────────────────────┘     │
                          │                                            │
  ┌──────────┐            │  ┌──────────────────────────────────┐     │
  │ Hangfire │────────────┼─►│  UC-13 Poll Health Checks (5 min)│     │
  │  (Jobs)  │            │  ├──────────────────────────────────┤     │
  └──────────┘            │  │  UC-14 Aggregate Metrics (1 min) │     │
                          │  ├──────────────────────────────────┤     │
                          │  │  UC-15 Cleanup Stale Alerts(day) │     │
                          │  └──────────────────────────────────┘     │
                          └──────────────────────────────────────────┘
```

### 5.2 Use Case Details

| UC | Name | Actor | Precondition | Main Flow | Postcondition |
|----|------|-------|-------------|-----------|---------------|
| UC-01 | Register Account | Visitor | None | POST `/api/auth/register` with email + password → Identity creates user | User account created |
| UC-02 | Login | Visitor | Account exists | POST `/api/auth/login` with credentials → JWT returned | JWT stored in browser localStorage |
| UC-03 | View Dashboard | Operator/Admin | Authenticated | GET hosts, latest metrics, open alerts | Dashboard displayed |
| UC-04 | Register Host | Operator/Admin | Authenticated | POST `/api/hosts` with name, IP, OS, tags | Host record persisted |
| UC-05 | View Host Details | Operator/Admin | Host exists | GET `/api/hosts/{id}` | Host + service nodes returned |
| UC-06 | Add Service Node | Operator/Admin | Host exists | POST `/api/servicenodes` with name, type, hostId, port | Service node persisted |
| UC-07 | Ingest Metric | Operator/Admin/Agent | Authenticated | POST `/api/metrics/ingest` → persisted + `MetricReceivedEvent` published → SignalR broadcast | Metric stored; real-time push to clients |
| UC-08 | Query Metrics | Operator/Admin | Authenticated | GET `/api/metrics/host/{id}?metricType=&from=&to=` | Filtered metric list returned |
| UC-09 | Create Alert | Operator/Admin | Authenticated | POST `/api/alerts` → persisted + `AlertCreatedEvent` published → SignalR broadcast | Alert stored; real-time push to clients |
| UC-10 | Acknowledge/Resolve Alert | Operator/Admin | Alert is Open/Acknowledged | PATCH `/api/alerts/{id}/status` with status | Alert status, timestamps, userId updated |
| UC-11 | View Reports | Operator/Admin | Authenticated | Navigate to `/reports` | Read-only summary displayed |
| UC-12 | Manage Settings | Admin | Authenticated + Admin role | Navigate to `/settings` | Settings page displayed |
| UC-13 | Poll Health Checks | Hangfire | Running | `HealthCheckPollingJob.ExecuteAsync()` (every 5 min) | Host/node statuses updated |
| UC-14 | Aggregate Metrics | Hangfire | Running | `MetricAggregationJob.ExecuteAsync()` (every 1 min) | Aggregated metric summaries computed |
| UC-15 | Cleanup Stale Alerts | Hangfire | Running | `StaleAlertCleanupJob.ExecuteAsync()` (daily) | Old open alerts transitioned/deleted |

---

## 6. Sequence Diagrams

### 6.1 User Login

```
Browser           API (/api/auth/login)     AuthService        Identity DB
   │                      │                      │                   │
   │── POST /login ───────►│                      │                   │
   │                      │── LoginCommand ──────►│                   │
   │                      │                      │── FindByEmailAsync►│
   │                      │                      │◄── ApplicationUser─│
   │                      │                      │── CheckPasswordSignIn                   │
   │                      │                      │◄── SignInResult    │
   │                      │                      │── BuildJwtToken()  │
   │                      │◄── LoginResultDto ───│                   │
   │◄── 200 { token } ────│                      │                   │
```

### 6.2 Ingest Metric & Real-time Push

```
Agent/Client     API (/api/metrics/ingest)   MediatR    MetricRepo   MediatR Publisher   SignalR MetricHub   Browser Dashboard
     │                    │                     │            │                │                   │                  │
     │── POST /ingest ────►│                     │            │                │                   │                  │
     │                    │── IngestMetricCmd ──►│            │                │                   │                  │
     │                    │                     │── AddAsync►│                │                   │                  │
     │                    │                     │◄── ok ─────│                │                   │                  │
     │                    │                     │── Publish(MetricReceivedEvent) ─────────────────►│                  │
     │                    │                     │            │                │── BroadcastMetric ─►│                  │
     │                    │                     │            │                │                   │── MetricReceived─►│
     │◄── 200 MetricDto ──│                     │            │                │                   │                  │
```

### 6.3 Create Alert & Acknowledge

```
Operator        API (/api/alerts)         MediatR        AlertRepo        SignalR AlertHub     Browser Dashboard
    │                  │                     │                │                    │                  │
    │── POST /alerts ──►│                     │                │                    │                  │
    │                  │── CreateAlertCmd ───►│                │                    │                  │
    │                  │                     │── AddAsync ────►│                    │                  │
    │                  │                     │── Publish(AlertCreatedEvent) ────────►│                  │
    │                  │                     │                │                    │── AlertCreated ───►│
    │◄── 200 AlertDto ─│                     │                │                    │                  │
    │                  │                     │                │                    │                  │
    │─ PATCH /{id}/status ──────────────────►│                │                    │                  │
    │    { status: Acknowledged }            │── UpdateAsync ─►│                    │                  │
    │◄── 200 AlertDto ──────────────────────│                │                    │                  │
```

### 6.4 Register Host

```
Operator      API (/api/hosts)        MediatR         HostRepo         PostgreSQL
    │                │                   │                │                  │
    │── POST /hosts ─►│                   │                │                  │
    │                │── CreateHostCmd ──►│                │                  │
    │                │                   │── ValidateAsync │                  │
    │                │                   │── ExistsAsync ─►│                  │
    │                │                   │◄── false ───────│                  │
    │                │                   │── AddAsync ─────►│── INSERT ────────►│
    │                │                   │◄── ok ──────────│◄── ok ───────────│
    │                │◄── HostDto ───────│                │                  │
    │◄── 201 ────────│                   │                │                  │
```

### 6.5 SignalR Client Connection

```
Browser (Angular)       SignalR (MetricHub/AlertHub)       JWT Middleware
       │                           │                             │
       │── HubConnectionBuilder ──►│                             │
       │   withUrl("/hubs/metrics   │                             │
       │      ?access_token=<jwt>") │                             │
       │                           │── ValidateToken ────────────►│
       │                           │◄── Authenticated ───────────│
       │── JoinHostGroup(hostId) ──►│                             │
       │◄── Connection established ─│                             │
       │◄── MetricReceived events ──│ (pushed on ingest)          │
```

---

## 7. Backend Setup

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [PostgreSQL 14+](https://www.postgresql.org/download/) (or Docker)
- (Optional) [Docker Desktop](https://www.docker.com/products/docker-desktop/) for running PostgreSQL

### 7.1 Start PostgreSQL (Docker)

```bash
# Main application database
docker run -d \
  --name smartops-postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=smartops \
  -p 5432:5432 \
  postgres:16

# Hangfire database (can be the same instance, different DB)
docker exec -it smartops-postgres \
  psql -U postgres -c "CREATE DATABASE smartops_hangfire;"
```

### 7.2 Configure Secrets

The project ships with placeholder values in `appsettings.json`. Override them **before** running:

```bash
# Navigate to API project
cd src/SmartOpsMonitoring.Api

# Set JWT signing key (development)
dotnet user-secrets set "Jwt:Key" "SuperSecretKey_AtLeast32Characters!!"

# Optional: override connection strings
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=5432;Database=smartops;Username=postgres;Password=postgres"
dotnet user-secrets set "ConnectionStrings:HangfireConnection" \
  "Host=localhost;Port=5432;Database=smartops_hangfire;Username=postgres;Password=postgres"
```

For production, use environment variables:

```bash
export Jwt__Key="your-production-secret"
export ConnectionStrings__DefaultConnection="Host=...;Database=smartops;..."
export ConnectionStrings__HangfireConnection="Host=...;Database=smartops_hangfire;..."
```

### 7.3 Build the Solution

```bash
# From repository root
dotnet build SmartOpsMonitoring.sln
```

### 7.4 Run Database Migrations

Migrations run **automatically** on startup. To run them manually:

```bash
dotnet tool install --global dotnet-ef   # install EF CLI if not already installed

dotnet ef database update \
  --project src/SmartOpsMonitoring.Infrastructure \
  --startup-project src/SmartOpsMonitoring.Api
```

### 7.5 Start the API

```bash
dotnet run --project src/SmartOpsMonitoring.Api
```

**Default URLs:**

| Endpoint | URL |
|----------|-----|
| Swagger UI | `http://localhost:5143/swagger` |
| Hangfire Dashboard | `http://localhost:5143/hangfire` |
| MetricHub (SignalR) | `ws://localhost:5143/hubs/metrics` |
| AlertHub (SignalR) | `ws://localhost:5143/hubs/alerts` |

### 7.6 API Quick Reference

| Controller | Method | Route | Description |
|-----------|--------|-------|-------------|
| Auth | POST | `/api/auth/register` | Register a new user |
| Auth | POST | `/api/auth/login` | Login, receive JWT |
| Hosts | GET | `/api/hosts` | List all hosts |
| Hosts | GET | `/api/hosts/{id}` | Get host by ID |
| Hosts | POST | `/api/hosts` | Create a host |
| ServiceNodes | GET | `/api/servicenodes?hostId={id}` | List service nodes for a host |
| ServiceNodes | POST | `/api/servicenodes` | Create a service node |
| Metrics | POST | `/api/metrics/ingest` | Ingest a metric |
| Metrics | GET | `/api/metrics/host/{id}` | Query metrics for host |
| Alerts | GET | `/api/alerts` | List alerts (filterable) |
| Alerts | POST | `/api/alerts` | Create an alert |
| Alerts | PATCH | `/api/alerts/{id}/status` | Update alert status |

---

## 8. Frontend Setup

### Prerequisites

- [Node.js 20+](https://nodejs.org/)
- [npm 10+](https://www.npmjs.com/)
- Angular CLI 19+: `npm install -g @angular/cli`

### 8.1 Install Dependencies

```bash
cd smart-ops-ui
npm install
```

### 8.2 Configure the Proxy

The Angular dev server proxies all `/api` and `/hubs` calls to the backend. The proxy is pre-configured in `smart-ops-ui/proxy.conf.json`:

```json
{
  "/api": {
    "target": "http://localhost:5143",
    "secure": false,
    "changeOrigin": true
  },
  "/hubs": {
    "target": "http://localhost:5143",
    "secure": false,
    "changeOrigin": true,
    "ws": true
  }
}
```

If your backend runs on a different port, update `target` accordingly.

### 8.3 Start the Development Server

```bash
cd smart-ops-ui
npm start
# or
ng serve
```

Open your browser at **`http://localhost:4200`**.

### 8.4 Build for Production

```bash
ng build --configuration=production
```

The build output is placed in `smart-ops-ui/dist/smart-ops-ui/`.

### 8.5 Frontend Routes

| Route | Guard | Description |
|-------|-------|-------------|
| `/auth/login` | None | Login page |
| `/auth/register` | None | Registration page |
| `/dashboard` | AuthGuard | Overview dashboard |
| `/hosts` | AuthGuard | Host list + detail |
| `/metrics` | AuthGuard | Metrics viewer with charts |
| `/alerts` | AuthGuard | Alert list + status update |
| `/reports` | AuthGuard | Read-only reports |
| `/settings` | AuthGuard + RoleGuard (Admin) | Admin settings |

---

## 9. Test Data Migration Script

Run this script against the `smartops` PostgreSQL database **after** the application has started at least once (so EF migrations have created all tables).

> **Note:** The `AspNetUsers`, `AspNetRoles`, and `AspNetUserRoles` tables are managed by ASP.NET Core Identity. User passwords below use BCrypt hashes equivalent to `Admin@123456` and `Operator@123456`.  
> Alternatively, use the `/api/auth/register` endpoint to create real Identity-managed accounts and then promote them manually as shown at the end of this script.

```sql
-- ============================================================
-- SmartOps Monitoring — Test Data Seed Script
-- Target DB : smartops
-- Run after first application startup (EF migrations applied)
-- ============================================================

BEGIN;

-- ─────────────────────────────────────────────────────────────
-- 1. Users (ASP.NET Core Identity)
-- ─────────────────────────────────────────────────────────────
-- NOTE: Use /api/auth/register to create real hashed passwords,
-- then run the promotion SQL below.  The example UUIDs match
-- the promotion block at the bottom.

-- Roles (created automatically by Program.cs on startup,
-- but included here for reference/standalone seeds)
INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
VALUES
  ('role-admin-0001-0000-0000-000000000001', 'Admin',    'ADMIN',    gen_random_uuid()::text),
  ('role-oper-0002-0000-0000-000000000002', 'Operator', 'OPERATOR', gen_random_uuid()::text)
ON CONFLICT ("Id") DO NOTHING;

-- ─────────────────────────────────────────────────────────────
-- 2. Hosts
-- ─────────────────────────────────────────────────────────────
INSERT INTO "Hosts" ("Id", "Name", "IpAddress", "OsType", "Status", "Tags", "CreatedAt", "UpdatedAt")
VALUES
  ('a0000000-0000-0000-0000-000000000001', 'web-server-01',  '192.168.1.10', 'Linux',   1, 'web,production',      NOW(), NOW()),
  ('a0000000-0000-0000-0000-000000000002', 'db-server-01',   '192.168.1.20', 'Linux',   1, 'database,production', NOW(), NOW()),
  ('a0000000-0000-0000-0000-000000000003', 'cache-server-01','192.168.1.30', 'Linux',   1, 'cache,production',    NOW(), NOW()),
  ('a0000000-0000-0000-0000-000000000004', 'win-server-01',  '192.168.1.40', 'Windows', 2, 'windows,staging',     NOW(), NOW()),
  ('a0000000-0000-0000-0000-000000000005', 'dev-machine-01', '10.0.0.5',     'macOS',   3, 'dev,maintenance',     NOW(), NOW())
ON CONFLICT ("Id") DO NOTHING;

-- ─────────────────────────────────────────────────────────────
-- 3. Service Nodes
-- ─────────────────────────────────────────────────────────────
INSERT INTO "ServiceNodes" ("Id", "Name", "Type", "HostId", "Status", "Port", "CreatedAt", "UpdatedAt")
VALUES
  ('b0000000-0000-0000-0000-000000000001', 'nginx',         'web',      'a0000000-0000-0000-0000-000000000001', 1, 80,   NOW(), NOW()),
  ('b0000000-0000-0000-0000-000000000002', 'api-dotnet',    'web',      'a0000000-0000-0000-0000-000000000001', 1, 5000, NOW(), NOW()),
  ('b0000000-0000-0000-0000-000000000003', 'postgresql',    'database', 'a0000000-0000-0000-0000-000000000002', 1, 5432, NOW(), NOW()),
  ('b0000000-0000-0000-0000-000000000004', 'redis',         'cache',    'a0000000-0000-0000-0000-000000000003', 1, 6379, NOW(), NOW()),
  ('b0000000-0000-0000-0000-000000000005', 'rabbitmq',      'queue',    'a0000000-0000-0000-0000-000000000003', 2, 5672, NOW(), NOW()),
  ('b0000000-0000-0000-0000-000000000006', 'iis-site',      'web',      'a0000000-0000-0000-0000-000000000004', 3, 443,  NOW(), NOW())
ON CONFLICT ("Id") DO NOTHING;

-- ─────────────────────────────────────────────────────────────
-- 4. Metrics  (last 2 hours, ~30 rows)
-- ─────────────────────────────────────────────────────────────
INSERT INTO "Metrics" ("Id", "HostId", "ServiceNodeId", "MetricType", "Value", "Unit", "Timestamp", "Labels", "CreatedAt", "UpdatedAt")
VALUES
  -- cpu_usage – web-server-01
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000001', NULL, 'cpu_usage',     12.4,  'percent', NOW() - INTERVAL '110 min', '{}', NOW(), NOW()),
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000001', NULL, 'cpu_usage',     18.7,  'percent', NOW() - INTERVAL '100 min', '{}', NOW(), NOW()),
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000001', NULL, 'cpu_usage',     25.1,  'percent', NOW() - INTERVAL '90 min',  '{}', NOW(), NOW()),
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000001', NULL, 'cpu_usage',     87.3,  'percent', NOW() - INTERVAL '80 min',  '{"alert":"spike"}', NOW(), NOW()),
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000001', NULL, 'cpu_usage',     45.6,  'percent', NOW() - INTERVAL '60 min',  '{}', NOW(), NOW()),
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000001', NULL, 'cpu_usage',     31.2,  'percent', NOW() - INTERVAL '30 min',  '{}', NOW(), NOW()),
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000001', NULL, 'cpu_usage',     22.0,  'percent', NOW() - INTERVAL '5 min',   '{}', NOW(), NOW()),
  -- memory_bytes – web-server-01
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000001', NULL, 'memory_bytes',  4294967296,  'bytes', NOW() - INTERVAL '60 min',  '{}', NOW(), NOW()),
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000001', NULL, 'memory_bytes',  5368709120,  'bytes', NOW() - INTERVAL '30 min',  '{}', NOW(), NOW()),
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000001', NULL, 'memory_bytes',  3221225472,  'bytes', NOW() - INTERVAL '5 min',   '{}', NOW(), NOW()),
  -- disk_read_bytes – web-server-01
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000001', NULL, 'disk_read_bytes', 10485760, 'bytes', NOW() - INTERVAL '30 min', '{}', NOW(), NOW()),
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000001', NULL, 'disk_read_bytes', 20971520, 'bytes', NOW() - INTERVAL '5 min',  '{}', NOW(), NOW()),

  -- cpu_usage – db-server-01
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000002', NULL, 'cpu_usage',     5.2,   'percent', NOW() - INTERVAL '60 min',  '{}', NOW(), NOW()),
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000002', NULL, 'cpu_usage',     8.9,   'percent', NOW() - INTERVAL '30 min',  '{}', NOW(), NOW()),
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000002', NULL, 'cpu_usage',     6.1,   'percent', NOW() - INTERVAL '5 min',   '{}', NOW(), NOW()),
  -- db_connections – postgresql service node
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000002', 'b0000000-0000-0000-0000-000000000003', 'db_connections', 42, 'count', NOW() - INTERVAL '30 min', '{"db":"smartops"}', NOW(), NOW()),
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000002', 'b0000000-0000-0000-0000-000000000003', 'db_connections', 67, 'count', NOW() - INTERVAL '5 min',  '{"db":"smartops"}', NOW(), NOW()),

  -- cache_hit_rate – cache-server-01
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000003', 'b0000000-0000-0000-0000-000000000004', 'cache_hit_rate', 97.4, 'percent', NOW() - INTERVAL '30 min', '{}', NOW(), NOW()),
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000003', 'b0000000-0000-0000-0000-000000000004', 'cache_hit_rate', 95.8, 'percent', NOW() - INTERVAL '5 min',  '{}', NOW(), NOW()),
  -- memory_bytes – cache-server-01
  (gen_random_uuid(), 'a0000000-0000-0000-0000-000000000003', NULL, 'memory_bytes',  2147483648, 'bytes', NOW() - INTERVAL '5 min', '{}', NOW(), NOW())
ON CONFLICT DO NOTHING;

-- ─────────────────────────────────────────────────────────────
-- 5. Alerts
-- ─────────────────────────────────────────────────────────────
INSERT INTO "Alerts" (
  "Id", "HostId", "ServiceNodeId",
  "Title", "Message", "Severity", "Status",
  "AcknowledgedAt", "ResolvedAt", "AcknowledgedByUserId",
  "CreatedAt", "UpdatedAt"
)
VALUES
  -- Critical open alert on web-server-01
  (
    'c0000000-0000-0000-0000-000000000001',
    'a0000000-0000-0000-0000-000000000001',
    NULL,
    'High CPU Usage',
    'CPU usage exceeded 85% on web-server-01 for more than 10 minutes.',
    2, 0,
    NULL, NULL, NULL,
    NOW() - INTERVAL '80 min', NOW() - INTERVAL '80 min'
  ),
  -- Warning acknowledged on web-server-01
  (
    'c0000000-0000-0000-0000-000000000002',
    'a0000000-0000-0000-0000-000000000001',
    'b0000000-0000-0000-0000-000000000001',
    'Nginx Slow Response',
    'Average nginx response time exceeded 2 seconds.',
    1, 1,
    NOW() - INTERVAL '50 min', NULL, 'operator-user-id',
    NOW() - INTERVAL '70 min', NOW() - INTERVAL '50 min'
  ),
  -- Info resolved on db-server-01
  (
    'c0000000-0000-0000-0000-000000000003',
    'a0000000-0000-0000-0000-000000000002',
    'b0000000-0000-0000-0000-000000000003',
    'High DB Connection Count',
    'PostgreSQL connection count exceeded 60.',
    0, 2,
    NOW() - INTERVAL '20 min', NOW() - INTERVAL '10 min', 'admin-user-id',
    NOW() - INTERVAL '30 min', NOW() - INTERVAL '10 min'
  ),
  -- Critical open on cache-server-01 (rabbitmq stopped)
  (
    'c0000000-0000-0000-0000-000000000004',
    'a0000000-0000-0000-0000-000000000003',
    'b0000000-0000-0000-0000-000000000005',
    'RabbitMQ Service Stopped',
    'RabbitMQ service node is in Stopped state.',
    2, 0,
    NULL, NULL, NULL,
    NOW() - INTERVAL '15 min', NOW() - INTERVAL '15 min'
  ),
  -- Warning open on win-server-01
  (
    'c0000000-0000-0000-0000-000000000005',
    'a0000000-0000-0000-0000-000000000004',
    NULL,
    'Host Offline',
    'win-server-01 is unreachable (status: Offline).',
    1, 0,
    NULL, NULL, NULL,
    NOW() - INTERVAL '5 min', NOW() - INTERVAL '5 min'
  )
ON CONFLICT ("Id") DO NOTHING;

COMMIT;

-- ─────────────────────────────────────────────────────────────
-- 6. Promote an existing Identity user to Admin role
--    Run AFTER registering via /api/auth/register
-- ─────────────────────────────────────────────────────────────
-- Replace <user-id> with the actual value from AspNetUsers.
--
-- SELECT "Id", "Email" FROM "AspNetUsers";
--
-- INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
-- SELECT '<user-id>', "Id" FROM "AspNetRoles" WHERE "Name" = 'Admin'
-- ON CONFLICT DO NOTHING;
```

### 9.1 Recommended Test Accounts

Create these accounts via the API after startup:

```bash
# Register Admin user
curl -X POST http://localhost:5143/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@smartops.local","password":"Admin@123456","displayName":"System Admin"}'

# Register Operator user
curl -X POST http://localhost:5143/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"operator@smartops.local","password":"Operator@123456","displayName":"Ops Operator"}'
```

Then promote the admin account in the database:

```sql
-- Get user ID
SELECT "Id", "Email" FROM "AspNetUsers" WHERE "Email" = 'admin@smartops.local';

-- Assign Admin role (replace <user-id>)
INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
SELECT '<user-id>', "Id"
FROM "AspNetRoles"
WHERE "Name" = 'Admin'
ON CONFLICT DO NOTHING;
```

### 9.2 End-to-End Smoke Test

```bash
# 1. Login and capture token
TOKEN=$(curl -s -X POST http://localhost:5143/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@smartops.local","password":"Admin@123456"}' \
  | jq -r '.token')

# 2. List hosts (should return seeded hosts after running seed script)
curl -s http://localhost:5143/api/hosts \
  -H "Authorization: Bearer $TOKEN" | jq .

# 3. Ingest a metric
curl -s -X POST http://localhost:5143/api/metrics/ingest \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "hostId": "a0000000-0000-0000-0000-000000000001",
    "metricType": "cpu_usage",
    "value": 33.5,
    "unit": "percent",
    "labels": {"region": "us-east"}
  }' | jq .

# 4. List open critical alerts
curl -s "http://localhost:5143/api/alerts?status=Open&severity=Critical" \
  -H "Authorization: Bearer $TOKEN" | jq .
```
