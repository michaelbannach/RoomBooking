using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Infrastructure.Data;
using RoomBooking.Infrastructure.Models;
using RoomBooking.Domain.Models;

namespace RoomBooking.Infrastructure.Seeding;

public static class DevelopmentSeeder
{
    private static bool _initialized;
    private static readonly object _lock = new();

    public static async Task SeedAsync(IServiceProvider services)
    {
        
        if (_initialized) return;
        lock (_lock)
        {
            if (_initialized) return;
            _initialized = true;
        }

        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

       
        if (!await db.Rooms.AnyAsync())
        {
            var rooms = new List<Room>
            {
                new Room { Name = "Raum 1", Capacity = 12 },
                new Room { Name = "Raum 2", Capacity = 8 },
                new Room { Name = "Raum 3", Capacity = 20 }
            };

            await db.Rooms.AddRangeAsync(rooms);
            await db.SaveChangesAsync();
        }

       
        var firstRoomId = await db.Rooms
            .OrderBy(r => r.Id)
            .Select(r => r.Id)
            .FirstAsync();

       
        const string email    = "seed_admin@test.local";
        const string userName = email;        
        const string password = "Test123!";

        var identityUser =
            await userManager.FindByEmailAsync(email)
            ?? await userManager.FindByNameAsync(userName);

        if (identityUser == null)
        {
            identityUser = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
                PhoneNumber = "0000000000"
            };

            var result = await userManager.CreateAsync(identityUser, password);

            if (!result.Succeeded)
            {
                var errorText = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Seeding of admin user failed: {errorText}");
            }
        }

       
        var domainUser = await db.Users
            .FirstOrDefaultAsync(u => u.IdentityUserId == identityUser.Id);

        if (domainUser == null)
        {
            domainUser = new User
            {
                IdentityUserId = identityUser.Id,
                FirstName = "Test",
                LastName = "User"
            };

            db.Users.Add(domainUser);
            await db.SaveChangesAsync();
        }
        
        if (!await db.Bookings.AnyAsync())
        {
            await db.Bookings.AddAsync(new Booking
            {
                StartDate = DateTime.Today.AddHours(10),
                EndDate = DateTime.Today.AddHours(12),
                UserId = domainUser.Id,
                RoomId = firstRoomId
            });

            await db.SaveChangesAsync();
        }
    }
}
