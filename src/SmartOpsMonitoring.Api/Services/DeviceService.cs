using SmartOpsMonitoring.Api.Data.Repositories;
using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.DTOs.Responses;
using SmartOpsMonitoring.Api.Models;

namespace SmartOpsMonitoring.Api.Services;

public class DeviceService : IDeviceService
{
    private readonly IRepository<Device> _deviceRepository;

    public DeviceService(IRepository<Device> deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<IEnumerable<DeviceResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var devices = await _deviceRepository.GetAllAsync(cancellationToken);
        return devices.Select(MapToResponse);
    }

    public async Task<DeviceResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var device = await _deviceRepository.GetByIdAsync(id, cancellationToken);
        return device == null ? null : MapToResponse(device);
    }

    public async Task<DeviceResponse> CreateAsync(CreateDeviceRequest request, CancellationToken cancellationToken = default)
    {
        var nameExists = await _deviceRepository.ExistsAsync(d => d.Name == request.Name, cancellationToken);
        if (nameExists)
            throw new InvalidOperationException($"Device with name '{request.Name}' already exists.");

        var device = new Device
        {
            Name = request.Name,
            Type = request.Type,
            Location = request.Location,
            IpAddress = request.IpAddress,
            Tags = request.Tags,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _deviceRepository.AddAsync(device, cancellationToken);
        return MapToResponse(created);
    }

    public async Task<DeviceResponse?> UpdateAsync(int id, UpdateDeviceRequest request, CancellationToken cancellationToken = default)
    {
        var device = await _deviceRepository.GetByIdAsync(id, cancellationToken);
        if (device == null) return null;

        if (request.Name != null) device.Name = request.Name;
        if (request.Type != null) device.Type = request.Type;
        if (request.Location != null) device.Location = request.Location;
        if (request.Status != null) device.Status = request.Status;
        if (request.IpAddress != null) device.IpAddress = request.IpAddress;
        if (request.Tags != null) device.Tags = request.Tags;
        if (request.IsActive.HasValue) device.IsActive = request.IsActive.Value;

        await _deviceRepository.UpdateAsync(device, cancellationToken);
        return MapToResponse(device);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var device = await _deviceRepository.GetByIdAsync(id, cancellationToken);
        if (device == null) return false;

        await _deviceRepository.DeleteAsync(device, cancellationToken);
        return true;
    }

    private static DeviceResponse MapToResponse(Device device) => new()
    {
        Id = device.Id,
        Name = device.Name,
        Type = device.Type,
        Location = device.Location,
        Status = device.Status,
        IpAddress = device.IpAddress,
        Tags = device.Tags,
        CreatedAt = device.CreatedAt,
        LastSeenAt = device.LastSeenAt,
        IsActive = device.IsActive
    };
}
