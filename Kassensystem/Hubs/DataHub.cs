using Microsoft.AspNetCore.SignalR;

namespace Kassensystem.Hubs;

public class DataHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}