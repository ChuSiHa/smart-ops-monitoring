# ── Build stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first for layer-cache optimised restore
COPY SmartOpsMonitoring.sln ./
COPY src/SmartOpsMonitoring.Domain/SmartOpsMonitoring.Domain.csproj             src/SmartOpsMonitoring.Domain/
COPY src/SmartOpsMonitoring.Application/SmartOpsMonitoring.Application.csproj   src/SmartOpsMonitoring.Application/
COPY src/SmartOpsMonitoring.Infrastructure/SmartOpsMonitoring.Infrastructure.csproj src/SmartOpsMonitoring.Infrastructure/
COPY src/SmartOpsMonitoring.Api/SmartOpsMonitoring.Api.csproj                   src/SmartOpsMonitoring.Api/

RUN dotnet restore src/SmartOpsMonitoring.Api/SmartOpsMonitoring.Api.csproj

# Copy all sources and publish
COPY . .
RUN dotnet publish src/SmartOpsMonitoring.Api/SmartOpsMonitoring.Api.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create a non-root user for security
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartOpsMonitoring.Api.dll"]
