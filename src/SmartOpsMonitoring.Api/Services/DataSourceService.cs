using SmartOpsMonitoring.Api.Data.Repositories;
using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.DTOs.Responses;
using SmartOpsMonitoring.Api.Models;

namespace SmartOpsMonitoring.Api.Services;

public class DataSourceService : IDataSourceService
{
    private readonly IRepository<DataSource> _dataSourceRepository;

    public DataSourceService(IRepository<DataSource> dataSourceRepository)
    {
        _dataSourceRepository = dataSourceRepository;
    }

    public async Task<IEnumerable<DataSourceResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sources = await _dataSourceRepository.GetAllAsync(cancellationToken);
        return sources.Select(MapToResponse);
    }

    public async Task<DataSourceResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var source = await _dataSourceRepository.GetByIdAsync(id, cancellationToken);
        return source == null ? null : MapToResponse(source);
    }

    public async Task<DataSourceResponse> CreateAsync(CreateDataSourceRequest request, CancellationToken cancellationToken = default)
    {
        var nameExists = await _dataSourceRepository.ExistsAsync(ds => ds.Name == request.Name, cancellationToken);
        if (nameExists)
            throw new InvalidOperationException($"Data source with name '{request.Name}' already exists.");

        var source = new DataSource
        {
            Name = request.Name,
            Type = request.Type,
            ConnectionString = request.ConnectionString,
            AdditionalConfig = request.AdditionalConfig,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _dataSourceRepository.AddAsync(source, cancellationToken);
        return MapToResponse(created);
    }

    public async Task<DataSourceResponse?> UpdateAsync(int id, UpdateDataSourceRequest request, CancellationToken cancellationToken = default)
    {
        var source = await _dataSourceRepository.GetByIdAsync(id, cancellationToken);
        if (source == null) return null;

        if (request.Name != null) source.Name = request.Name;
        if (request.Type != null) source.Type = request.Type;
        if (request.ConnectionString != null) source.ConnectionString = request.ConnectionString;
        if (request.AdditionalConfig != null) source.AdditionalConfig = request.AdditionalConfig;
        if (request.IsActive.HasValue) source.IsActive = request.IsActive.Value;

        await _dataSourceRepository.UpdateAsync(source, cancellationToken);
        return MapToResponse(source);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var source = await _dataSourceRepository.GetByIdAsync(id, cancellationToken);
        if (source == null) return false;

        await _dataSourceRepository.DeleteAsync(source, cancellationToken);
        return true;
    }

    public async Task<DataSourceResponse?> TestConnectionAsync(int id, CancellationToken cancellationToken = default)
    {
        var source = await _dataSourceRepository.GetByIdAsync(id, cancellationToken);
        if (source == null) return null;

        // Simulate connection test — a real implementation would attempt to connect
        // based on source.Type (e.g., SQL, InfluxDB, Prometheus, MQTT, etc.)
        bool testPassed;
        try
        {
            testPassed = !string.IsNullOrWhiteSpace(source.ConnectionString);
            await Task.Delay(100, cancellationToken); // simulate async test
        }
        catch
        {
            testPassed = false;
        }

        source.LastTestedAt = DateTime.UtcNow;
        source.LastTestPassed = testPassed;
        await _dataSourceRepository.UpdateAsync(source, cancellationToken);

        return MapToResponse(source);
    }

    private static DataSourceResponse MapToResponse(DataSource ds) => new()
    {
        Id = ds.Id,
        Name = ds.Name,
        Type = ds.Type,
        AdditionalConfig = ds.AdditionalConfig,
        IsActive = ds.IsActive,
        CreatedAt = ds.CreatedAt,
        LastTestedAt = ds.LastTestedAt,
        LastTestPassed = ds.LastTestPassed
    };
}
