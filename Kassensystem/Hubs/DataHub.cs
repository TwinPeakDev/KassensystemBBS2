using Microsoft.AspNetCore.SignalR;
using Kassensystem.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Kassensystem.Hubs;

public class DataHub : Hub
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public DataHub(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }


    public async Task SendMessage(string user, string message) 
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }


    public async Task SavePayment(Sold sold) 
    {
        var context = _contextFactory.CreateDbContext();

        await context.SellEntries.AddAsync(sold);
        await context.SaveChangesAsync();

        await context.DisposeAsync();

        await Clients.All.SendAsync("PaymentSaved");
    }

    
}