using Microsoft.EntityFrameworkCore;
using SmartOpsMonitoring.Api.Data;
using SmartOpsMonitoring.Api.Data.Repositories;
using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.DTOs.Responses;
using SmartOpsMonitoring.Api.Models;

namespace SmartOpsMonitoring.Api.Services;

public class MetricService : IMetricService
{
    private readonly IRepository<Metric> _metricRepository;
    private readonly IRepository<Device> _deviceRepository;
    private readonly AppDbContext _context;

    public MetricService(IRepository<Metric> metricRepository, IRepository<Device> deviceRepository, AppDbContext context)
    {
        _metricRepository = metricRepository;
        _deviceRepository = deviceRepository;
        _context = context;
    }

    public async Task<MetricResponse> IngestAsync(IngestMetricRequest request, CancellationToken cancellationToken = default)
    {
        var device = await _deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken)
            ?? throw new KeyNotFoundException($"Device {request.DeviceId} not found.");

        var metric = new Metric
        {
            DeviceId = request.DeviceId,
            MetricType = request.MetricType,
            Value = request.Value,
            Unit = request.Unit,
            Timestamp = request.Timestamp?.ToUniversalTime() ?? DateTime.UtcNow,
            Labels = request.Labels
        };

        device.LastSeenAt = DateTime.UtcNow;
        device.Status = "Online";
        await _deviceRepository.UpdateAsync(device, cancellationToken);

        var created = await _metricRepository.AddAsync(metric, cancellationToken);

        return new MetricResponse
        {
            Id = created.Id,
            DeviceId = created.DeviceId,
            DeviceName = device.Name,
            MetricType = created.MetricType,
            Value = created.Value,
            Unit = created.Unit,
            Timestamp = created.Timestamp,
            Labels = created.Labels
        };
    }

    public async Task<PagedMetricResponse> QueryAsync(MetricQueryRequest query, CancellationToken cancellationToken = default)
    {
        var q = _context.Metrics.Include(m => m.Device).AsQueryable();

        if (query.DeviceId.HasValue)
            q = q.Where(m => m.DeviceId == query.DeviceId.Value);

        if (!string.IsNullOrWhiteSpace(query.MetricType))
            q = q.Where(m => m.MetricType == query.MetricType);

        if (query.From.HasValue)
            q = q.Where(m => m.Timestamp >= query.From.Value.ToUniversalTime());

        if (query.To.HasValue)
            q = q.Where(m => m.Timestamp <= query.To.Value.ToUniversalTime());

        var totalCount = await q.CountAsync(cancellationToken);
        var pageSize = Math.Max(1, Math.Min(query.PageSize, 1000));
        var page = Math.Max(1, query.Page);
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var items = await q
            .OrderByDescending(m => m.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedMetricResponse
        {
            Items = items.Select(m => new MetricResponse
            {
                Id = m.Id,
                DeviceId = m.DeviceId,
                DeviceName = m.Device.Name,
                MetricType = m.MetricType,
                Value = m.Value,
                Unit = m.Unit,
                Timestamp = m.Timestamp,
                Labels = m.Labels
            }),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<IEnumerable<MetricResponse>> GetLatestByDeviceAsync(int deviceId, CancellationToken cancellationToken = default)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceId, cancellationToken)
            ?? throw new KeyNotFoundException($"Device {deviceId} not found.");

        var latest = await _context.Metrics
            .Where(m => m.DeviceId == deviceId)
            .GroupBy(m => m.MetricType)
            .Select(g => g.OrderByDescending(m => m.Timestamp).First())
            .ToListAsync(cancellationToken);

        return latest.Select(m => new MetricResponse
        {
            Id = m.Id,
            DeviceId = m.DeviceId,
            DeviceName = device.Name,
            MetricType = m.MetricType,
            Value = m.Value,
            Unit = m.Unit,
            Timestamp = m.Timestamp,
            Labels = m.Labels
        });
    }
}
