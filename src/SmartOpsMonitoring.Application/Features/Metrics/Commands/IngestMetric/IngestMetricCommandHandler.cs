namespace SmartOpsMonitoring.Application.Features.Metrics.Commands.IngestMetric;

/// <summary>
/// Handles <see cref="IngestMetricCommand"/> by persisting the metric and publishing a domain event.
/// </summary>
public class IngestMetricCommandHandler : ICommandHandler<IngestMetricCommand, MetricDto>
{
    private readonly IMetricRepository _metricRepository;
    private readonly IPublisher _publisher;

    /// <summary>
    /// Initialises a new instance of <see cref="IngestMetricCommandHandler"/>.
    /// </summary>
    /// <param name="metricRepository">The metric repository.</param>
    /// <param name="publisher">The MediatR publisher used to dispatch domain events.</param>
    public IngestMetricCommandHandler(IMetricRepository metricRepository, IPublisher publisher)
    {
        _metricRepository = metricRepository;
        _publisher = publisher;
    }

    /// <summary>
    /// Executes the command: creates and persists a <see cref="Metric"/> entity, then publishes
    /// a <see cref="MetricReceivedEvent"/>.
    /// </summary>
    /// <param name="request">The ingest command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="MetricDto"/> representing the persisted metric.</returns>
    public async Task<MetricDto> Handle(IngestMetricCommand request, CancellationToken cancellationToken)
    {
        var metric = request.Adapt<Metric>();
        await _metricRepository.AddAsync(metric, cancellationToken);

        // Publish domain event so subscribers (e.g. SignalR MetricHub) can push updates to clients.
        await _publisher.Publish(
            new MetricReceivedEvent(metric.Id, metric.HostId, metric.MetricType, metric.Value),
            cancellationToken);

        return metric.Adapt<MetricDto>();
    }

    /// <summary>
    /// Maps a <see cref="Metric"/> entity to a <see cref="MetricDto"/>.
    /// </summary>
    /// <param name="metric">The metric entity to map.</param>
    /// <returns>The mapped <see cref="MetricDto"/>.</returns>
    private static MetricDto MapToDto(Metric metric) => metric.Adapt<MetricDto>();
}
