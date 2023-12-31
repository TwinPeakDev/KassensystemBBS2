
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

using Kassensystem.Data.Database;
using Kassensystem.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddControllers();
builder.Services                    
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(); 

//Add the different Radzen services
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

// Add signal r response compression 
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

var inMemory = true;

if(inMemory)
    builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseInMemoryDatabase("TestDatabase"));
else
{
    var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
    builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
}


var app = builder.Build();

// Add response compression middleware
app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapHub<DataHub>("/datahub");
app.MapFallbackToPage("/_Host");

if (inMemory)
{
    var dbFactory = app.Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    var db = await dbFactory.CreateDbContextAsync();
    var rd = new Random();

    var products = new List<Product>();

    for (int i = 0; i < 10; i++)
    {
        products.Add(new Product()
        {
            Name = "Test_" + rd.Next(0,1000),
            PriceEuro = rd.Next(0,1000)
        });
    }

    await db.Products.AddRangeAsync(products);
    await db.SaveChangesAsync();

}

app.Run();