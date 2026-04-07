using SmartOpsMonitoring.Api.Data.Repositories;
using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.DTOs.Responses;
using SmartOpsMonitoring.Api.Models;

namespace SmartOpsMonitoring.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly IRepository<Dashboard> _dashboardRepository;

    public DashboardService(IRepository<Dashboard> dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<IEnumerable<DashboardResponse>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var dashboards = await _dashboardRepository.FindAsync(d => d.UserId == userId, cancellationToken);
        return dashboards.Select(MapToResponse);
    }

    public async Task<DashboardResponse?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var dashboards = await _dashboardRepository.FindAsync(d => d.Id == id && d.UserId == userId, cancellationToken);
        var dashboard = dashboards.FirstOrDefault();
        return dashboard == null ? null : MapToResponse(dashboard);
    }

    public async Task<DashboardResponse> CreateAsync(int userId, CreateDashboardRequest request, CancellationToken cancellationToken = default)
    {
        if (request.IsDefault)
        {
            var existing = await _dashboardRepository.FindAsync(d => d.UserId == userId && d.IsDefault, cancellationToken);
            foreach (var d in existing)
            {
                d.IsDefault = false;
                await _dashboardRepository.UpdateAsync(d, cancellationToken);
            }
        }

        var dashboard = new Dashboard
        {
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            ConfigJson = request.ConfigJson,
            IsDefault = request.IsDefault,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _dashboardRepository.AddAsync(dashboard, cancellationToken);
        return MapToResponse(created);
    }

    public async Task<DashboardResponse?> UpdateAsync(int id, int userId, UpdateDashboardRequest request, CancellationToken cancellationToken = default)
    {
        var dashboards = await _dashboardRepository.FindAsync(d => d.Id == id && d.UserId == userId, cancellationToken);
        var dashboard = dashboards.FirstOrDefault();
        if (dashboard == null) return null;

        if (request.IsDefault == true)
        {
            var existing = await _dashboardRepository.FindAsync(d => d.UserId == userId && d.IsDefault && d.Id != id, cancellationToken);
            foreach (var d in existing)
            {
                d.IsDefault = false;
                await _dashboardRepository.UpdateAsync(d, cancellationToken);
            }
        }

        if (request.Name != null) dashboard.Name = request.Name;
        if (request.Description != null) dashboard.Description = request.Description;
        if (request.ConfigJson != null) dashboard.ConfigJson = request.ConfigJson;
        if (request.IsDefault.HasValue) dashboard.IsDefault = request.IsDefault.Value;
        dashboard.UpdatedAt = DateTime.UtcNow;

        await _dashboardRepository.UpdateAsync(dashboard, cancellationToken);
        return MapToResponse(dashboard);
    }

    public async Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var dashboards = await _dashboardRepository.FindAsync(d => d.Id == id && d.UserId == userId, cancellationToken);
        var dashboard = dashboards.FirstOrDefault();
        if (dashboard == null) return false;

        await _dashboardRepository.DeleteAsync(dashboard, cancellationToken);
        return true;
    }

    private static DashboardResponse MapToResponse(Dashboard d) => new()
    {
        Id = d.Id,
        UserId = d.UserId,
        Name = d.Name,
        Description = d.Description,
        ConfigJson = d.ConfigJson,
        IsDefault = d.IsDefault,
        CreatedAt = d.CreatedAt,
        UpdatedAt = d.UpdatedAt
    };
}
