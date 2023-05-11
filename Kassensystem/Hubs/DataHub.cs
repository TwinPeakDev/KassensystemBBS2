using Microsoft.AspNetCore.SignalR;
using Kassensystem.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Kassensystem.Data.Database;

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
        var context = await _contextFactory.CreateDbContextAsync();

        context.SellEntries.Update(sold);
        
        await context.SaveChangesAsync();

        await context.DisposeAsync();

        await Clients.All.SendAsync("UpdatePayments");
    }

    public async Task SaveProduct(Product product)
    {
        var context = await _contextFactory.CreateDbContextAsync();

        context.Update(product);       
        
        await context.SaveChangesAsync();

        await context.DisposeAsync();
        await Clients.All.SendAsync("UpdateProducts");
    }

    public async Task RemoveProduct(Product product)
    {
        var context = await _contextFactory.CreateDbContextAsync();
        
        context.Remove(product);        
        await context.SaveChangesAsync();
        
        await context.DisposeAsync();
        await Clients.All.SendAsync("UpdateProducts");
    }

}