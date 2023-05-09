using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Kassensystem.Data;
using Kassensystem.Data.Database;
using Kassensystem.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
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


var connectionString = Environment.GetEnvironmentVariable("ConnectionString") ?? "Server=localhost;Database=db;Uid=root;Pwd=G7P!Mub@Y7ZwJu2Fm^ea;";
if(connectionString != "")
    builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
else
    builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseInMemoryDatabase("TestDatabase"));

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
app.MapHub<DataHub>("/datahub", o =>
{
    o.ApplicationMaxBufferSize = 200000;
    o.TransportMaxBufferSize = 200000;
});
app.MapFallbackToPage("/_Host");

app.Run();