namespace SmartOpsMonitoring.Infrastructure.Hubs;

/// <summary>
/// SignalR hub for streaming real-time metric data to connected clients.
/// </summary>
public class MetricHub : Hub
{
    /// <summary>
    /// Adds the calling client to a group scoped to a specific host, enabling targeted metric pushes.
    /// </summary>
    /// <param name="hostId">The host identifier used as the group name.</param>
    public async Task JoinHostGroup(string hostId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, hostId);

    /// <summary>
    /// Removes the calling client from the host group.
    /// </summary>
    /// <param name="hostId">The host identifier used as the group name.</param>
    public async Task LeaveHostGroup(string hostId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, hostId);

    /// <summary>
    /// Broadcasts a metric payload to all clients in the specified host group.
    /// </summary>
    /// <param name="hubContext">The SignalR hub context used to push data.</param>
    /// <param name="hostId">The target host group identifier.</param>
    /// <param name="payload">The metric data to broadcast.</param>
    public static async Task BroadcastMetric(IHubContext<MetricHub> hubContext, string hostId, object payload)
        => await hubContext.Clients.Group(hostId).SendAsync("MetricReceived", payload);
}
