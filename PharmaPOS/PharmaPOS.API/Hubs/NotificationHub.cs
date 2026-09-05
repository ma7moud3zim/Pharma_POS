using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PharmaPOS.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var role = Context.User?.FindFirst("realm_access")?.Value;

        // Add user to role-based groups
        if (Context.User?.IsInRole("Admin") == true)
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");

        if (Context.User?.IsInRole("Pharmacist") == true)
            await Groups.AddToGroupAsync(Context.ConnectionId, "Pharmacists");

        if (Context.User?.IsInRole("Manager") == true)
            await Groups.AddToGroupAsync(Context.ConnectionId, "Managers");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}