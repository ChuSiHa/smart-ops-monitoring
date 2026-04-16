using Mapster;

namespace SmartOpsMonitoring.Application.Mappings;

/// <summary>
/// Centralized Mapster mapping configuration for the Application layer.
/// Call <see cref="RegisterMappings"/> once at application startup to apply all custom rules.
/// </summary>
public static class MappingConfig
{
    /// <summary>
    /// Registers all custom mapping rules with the global <see cref="TypeAdapterConfig"/>.
    /// Rules are applied on top of Mapster's default convention-based property matching.
    /// </summary>
    public static void RegisterMappings()
    {
        // CreateAlertCommand → Alert
        // Severity is stored as a string on the command but as an enum on the entity;
        // Status defaults to Open when a new alert is created.
        TypeAdapterConfig<Features.Alerts.Commands.CreateAlert.CreateAlertCommand, Alert>
            .NewConfig()
            .Map(dest => dest.Status, src => AlertStatus.Open);

        // IngestMetricCommand → Metric
        // Timestamp is optional on the command; default to UtcNow when absent.
        TypeAdapterConfig<Features.Metrics.Commands.IngestMetric.IngestMetricCommand, Metric>
            .NewConfig()
            .Map(dest => dest.Timestamp, src => src.Timestamp ?? DateTime.UtcNow);
    }
}
