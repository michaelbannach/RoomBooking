using Microsoft.AspNetCore.Identity;
using RoomBooking.Domain.Models;
using RoomBooking.Infrastructure.Data;

namespace RoomBooking.Infrastructure.Seeding;

public static class DevelopmentSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Employee>>();

        // Room seeden
        if (!db.Rooms.Any())
        {
            db.Rooms.Add(new Room { Name = "Raum 1", Capacity = 12 });
            db.SaveChanges();
        }
        var roomId = db.Rooms.First().Id;

        // User seeden
        Employee user;
        if (!userManager.Users.Any())
        {
            user = new Employee
            {
                UserName = "testuser",
                Email = "testuser@test.com",
                EmailConfirmed = true,
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = "00000000"
            };
            
            var result = await userManager.CreateAsync(user, "Test123!");
            if (!result.Succeeded)
            {
                // Fehlerbehandlung zur Not hier ergänzen
                throw new Exception("User konnte nicht angelegt werden: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            user = userManager.Users.First();
        }
        var employeeId = user.Id;

        // Booking seeden (nur wenn noch keine existiert)
        if (!db.Bookings.Any())
        {
            db.Bookings.Add(new Booking
            {
                StartDate = DateTime.Today.AddHours(10),
                EndDate = DateTime.Today.AddHours(12),
                EmployeeId = employeeId,
                RoomId = roomId
            });
            db.SaveChanges();
        }
    }
}