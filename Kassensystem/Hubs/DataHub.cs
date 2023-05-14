/*
Copyright (C) 2023  
Elias Stepanik: https://github.com/eliasstepanik
Olivia Streun: https://github.com/nnuuvv

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see https://www.gnu.org/licenses/.
*/

using Microsoft.AspNetCore.SignalR;
using Kassensystem.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Kassensystem.Data.Database;

namespace Kassensystem.Hubs;

public class DataHub : Hub
{
    private readonly IDbContextFactory<DataContext> _contextFactory;

    public DataHub(IDbContextFactory<DataContext> contextFactory)
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