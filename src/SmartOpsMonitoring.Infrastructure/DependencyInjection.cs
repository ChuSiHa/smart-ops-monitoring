namespace SmartOpsMonitoring.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure layer services with the DI container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Infrastructure layer services including EF Core, Identity, repositories,
    /// Hangfire, and SignalR.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IHostRepository, HostRepository>();
        services.AddScoped<IServiceNodeRepository, ServiceNodeRepository>();
        services.AddScoped<IMetricRepository, MetricRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(c =>
                c.UseNpgsqlConnection(configuration.GetConnectionString("HangfireConnection"))));

        services.AddHangfireServer();

        services.AddSignalR();

        return services;
    }
}
