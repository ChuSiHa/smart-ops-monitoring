using Microsoft.AspNetCore.SignalR;

namespace SmartOpsMonitoring.Infrastructure.Hubs;

/// <summary>
/// SignalR hub for streaming real-time alert notifications to connected clients.
/// </summary>
public class AlertHub : Hub
{
    /// <summary>
    /// Adds the calling client to a group scoped to a specific host for alert notifications.
    /// </summary>
    /// <param name="hostId">The host identifier used as the group name.</param>
    public async Task JoinHostGroup(string hostId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, hostId);

    /// <summary>
    /// Removes the calling client from the host alert group.
    /// </summary>
    /// <param name="hostId">The host identifier used as the group name.</param>
    public async Task LeaveHostGroup(string hostId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, hostId);
}
