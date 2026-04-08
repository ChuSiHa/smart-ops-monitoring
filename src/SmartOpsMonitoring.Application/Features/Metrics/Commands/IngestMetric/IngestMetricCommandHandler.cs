using MediatR;
using SmartOpsMonitoring.Application.DTOs;
using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Events;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Application.Features.Metrics.Commands.IngestMetric;

/// <summary>
/// Handles <see cref="IngestMetricCommand"/> by persisting the metric and publishing a domain event.
/// </summary>
public class IngestMetricCommandHandler : IRequestHandler<IngestMetricCommand, MetricDto>
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
        var metric = new Metric
        {
            HostId = request.HostId,
            ServiceNodeId = request.ServiceNodeId,
            MetricType = request.MetricType,
            Value = request.Value,
            Unit = request.Unit,
            Timestamp = request.Timestamp ?? DateTime.UtcNow,
            Labels = request.Labels
        };

        await _metricRepository.AddAsync(metric, cancellationToken);

        await _publisher.Publish(
            new MetricReceivedEvent(metric.Id, metric.HostId, metric.MetricType, metric.Value),
            cancellationToken);

        return MapToDto(metric);
    }

    private static MetricDto MapToDto(Metric metric) => new()
    {
        Id = metric.Id,
        HostId = metric.HostId,
        ServiceNodeId = metric.ServiceNodeId,
        MetricType = metric.MetricType,
        Value = metric.Value,
        Unit = metric.Unit,
        Timestamp = metric.Timestamp,
        Labels = metric.Labels
    };
}
