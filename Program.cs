using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using RoomBookingC.Data;
using RoomBookingC.Models;

var builder = WebApplication.CreateBuilder(args);

// Authentication & Authorization
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization();

// MVC & JSON
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Datenbankkontext
builder.Services.AddDbContext<RoomBookContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// MIGRATIONS & INITIAL-DATEN MIT RETRY
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RoomBookContext>();
    
    const int maxRetries = 10;
    var retries = maxRetries;

    while (retries > 0)
    {
        try
        {
            db.Database.Migrate(); // Migrations anwenden

            // Seed-Daten, falls leer
            if (!db.Rooms.Any())
            {
                db.Rooms.AddRange(
                    new Room { Name = "Besprechungsraum 1" },
                    new Room { Name = "Besprechungsraum 2" },
                    new Room { Name = "Besprechungsraum 3" }
                );
                db.SaveChanges();
            }

            break; // Erfolgreich
        }
        catch (Exception ex)
        {
            retries--;
            Console.WriteLine($" Datenbankverbindung fehlgeschlagen ({maxRetries - retries}/{maxRetries}). Fehler: {ex.Message}");
            Thread.Sleep(3000); // 3 Sekunden warten
        }
    }
}

// Middlewares
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
