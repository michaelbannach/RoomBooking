using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


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

        // ROOM seeden
        if (!await db.Rooms.AnyAsync())
        {
            await db.Rooms.AddAsync(new Room
            {
                Name = "Raum 1",
                Capacity = 12
            });

            await db.SaveChangesAsync();
        }

        var roomId = await db.Rooms
            .Select(r => r.Id)
            .FirstAsync();

        // USER seeden
        Employee user;

        if (!await userManager.Users.AnyAsync())
        {
            user = new Employee
            {
                UserName = "testuser",
                Email = "testuser@test.com",
                EmailConfirmed = true,
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = "0000000000"
            };

            var result = await userManager.CreateAsync(user, "Test123!");

            if (!result.Succeeded)
            {
                throw new Exception("User konnte nicht angelegt werden: " +
                                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            user = await userManager.Users.FirstAsync();
        }

        var employeeId = user.Id;

        // BOOKING seeden
        if (!await db.Bookings.AnyAsync())
        {
            await db.Bookings.AddAsync(new Booking
            {
                StartDate = DateTime.Today.AddHours(10),
                EndDate = DateTime.Today.AddHours(12),
                EmployeeId = employeeId,
                RoomId = roomId
            });

            await db.SaveChangesAsync();
        }
    }
}