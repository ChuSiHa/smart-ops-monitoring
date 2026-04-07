# Smart Ops Monitoring — Backend API

ASP.NET Core 10 Web API for the Smart Operations Monitoring platform.

## Tech stack

- **.NET 10** / ASP.NET Core Web API
- **Entity Framework Core 10** + SQLite (swap to SQL Server by changing the connection string)
- **JWT Bearer** authentication (BCrypt password hashing)
- **Swashbuckle / Swagger UI** with Bearer token support
- Generic repository pattern + service layer

## Getting started

### 1. Configure secrets

**Never commit real secrets.** The `appsettings.json` file ships with a placeholder JWT key. Override it before running in any non-local environment:

```bash
# Via environment variable (recommended)
export Jwt__Key="your-256-bit-secret-key-here"

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

The SQLite database is created and migrated automatically on first startup.

Swagger UI is available at: `https://localhost:{port}/swagger`

## API overview

| Controller     | Base route          | Roles required       | Description                        |
|----------------|---------------------|----------------------|------------------------------------|
| Auth           | `/api/auth`         | Public / Authenticated | Register, login, current user      |
| Devices        | `/api/devices`      | All / Admin+Operator | Device CRUD                        |
| Metrics        | `/api/metrics`      | All authenticated    | Ingest & query telemetry metrics   |
| Alerts         | `/api/alerts`       | All / Admin+Operator | Alert lifecycle management         |
| Dashboards     | `/api/dashboards`   | All authenticated    | Per-user dashboard configuration   |
| DataSources    | `/api/datasources`  | Admin only           | External data source management    |

## Project structure

```
src/
└── SmartOpsMonitoring.Api/
    ├── Controllers/       # HTTP layer
    ├── Data/
    │   ├── AppDbContext.cs
    │   ├── Migrations/
    │   └── Repositories/  # Generic IRepository<T>
    ├── DTOs/
    │   ├── Requests/
    │   └── Responses/
    ├── Middleware/        # Global exception handler
    ├── Models/            # Domain entities
    ├── Services/          # Business logic
    ├── appsettings.json
    └── Program.cs
```

## User roles

| Role       | Description                                      |
|------------|--------------------------------------------------|
| `Admin`    | Full access including device deletion, data sources |
| `Operator` | Create/update devices, create/acknowledge alerts |
| `Viewer`   | Read-only access                                 |
