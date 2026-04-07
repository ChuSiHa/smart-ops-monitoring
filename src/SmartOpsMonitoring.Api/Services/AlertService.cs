using Microsoft.EntityFrameworkCore;
using SmartOpsMonitoring.Api.Data;
using SmartOpsMonitoring.Api.Data.Repositories;
using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.DTOs.Responses;
using SmartOpsMonitoring.Api.Models;

namespace SmartOpsMonitoring.Api.Services;

public class AlertService : IAlertService
{
    private readonly IRepository<Alert> _alertRepository;
    private readonly IRepository<Device> _deviceRepository;
    private readonly AppDbContext _context;

    public AlertService(IRepository<Alert> alertRepository, IRepository<Device> deviceRepository, AppDbContext context)
    {
        _alertRepository = alertRepository;
        _deviceRepository = deviceRepository;
        _context = context;
    }

    public async Task<IEnumerable<AlertResponse>> GetAllAsync(string? status = null, string? severity = null, CancellationToken cancellationToken = default)
    {
        var q = _context.Alerts.Include(a => a.Device).AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            q = q.Where(a => a.Status == status);

        if (!string.IsNullOrWhiteSpace(severity))
            q = q.Where(a => a.Severity == severity);

        var alerts = await q.OrderByDescending(a => a.CreatedAt).ToListAsync(cancellationToken);
        return alerts.Select(MapToResponse);
    }

    public async Task<AlertResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var alert = await _context.Alerts
            .Include(a => a.Device)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return alert == null ? null : MapToResponse(alert);
    }

    public async Task<AlertResponse> CreateAsync(CreateAlertRequest request, CancellationToken cancellationToken = default)
    {
        var deviceExists = await _deviceRepository.ExistsAsync(d => d.Id == request.DeviceId, cancellationToken);
        if (!deviceExists)
            throw new KeyNotFoundException($"Device {request.DeviceId} not found.");

        var alert = new Alert
        {
            DeviceId = request.DeviceId,
            Title = request.Title,
            Message = request.Message,
            Severity = request.Severity,
            Status = "Open",
            CreatedAt = DateTime.UtcNow
        };

        var created = await _alertRepository.AddAsync(alert, cancellationToken);

        var alertWithDevice = await _context.Alerts
            .Include(a => a.Device)
            .FirstAsync(a => a.Id == created.Id, cancellationToken);

        return MapToResponse(alertWithDevice);
    }

    public async Task<AlertResponse?> UpdateAsync(int id, UpdateAlertRequest request, int currentUserId, CancellationToken cancellationToken = default)
    {
        var alert = await _context.Alerts
            .Include(a => a.Device)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (alert == null) return null;

        if (request.Status != null)
        {
            alert.Status = request.Status;
            if (request.Status == "Acknowledged" && !alert.AcknowledgedAt.HasValue)
            {
                alert.AcknowledgedAt = DateTime.UtcNow;
                alert.AcknowledgedByUserId = currentUserId;
            }
            else if (request.Status == "Resolved" && !alert.ResolvedAt.HasValue)
            {
                alert.ResolvedAt = DateTime.UtcNow;
            }
        }

        await _alertRepository.UpdateAsync(alert, cancellationToken);
        return MapToResponse(alert);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var alert = await _alertRepository.GetByIdAsync(id, cancellationToken);
        if (alert == null) return false;

        await _alertRepository.DeleteAsync(alert, cancellationToken);
        return true;
    }

    private static AlertResponse MapToResponse(Alert alert) => new()
    {
        Id = alert.Id,
        DeviceId = alert.DeviceId,
        DeviceName = alert.Device.Name,
        Title = alert.Title,
        Message = alert.Message,
        Severity = alert.Severity,
        Status = alert.Status,
        CreatedAt = alert.CreatedAt,
        AcknowledgedAt = alert.AcknowledgedAt,
        ResolvedAt = alert.ResolvedAt,
        AcknowledgedByUserId = alert.AcknowledgedByUserId
    };
}
