using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
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

        // 1) ROOM seeden
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

        
        const string userName = "seed_admin";         
        const string email    = "seed_admin@test.local";
        const string password = "Test123!";

        var existingIdentityUser =
            await userManager.FindByNameAsync(userName)
            ?? await userManager.FindByEmailAsync(email);

        if (existingIdentityUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
                PhoneNumber = "0000000000"
            };

            try
            {
                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    existingIdentityUser = user;
                }
                else if (result.Errors.Any(e =>
                             e.Code == "DuplicateUserName" ||
                             e.Code == "DuplicateEmail"))
                {
                    
                    existingIdentityUser =
                        await userManager.FindByNameAsync(userName)
                        ?? await userManager.FindByEmailAsync(email);
                }
                else
                {
                    throw new Exception(
                        "ApplicationUser couldn´t be created: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is SqlException sqlEx &&
                sqlEx.Message.Contains("UserNameIndex", StringComparison.OrdinalIgnoreCase))
            {
              
                existingIdentityUser =
                    await userManager.FindByNameAsync(userName)
                    ?? await userManager.FindByEmailAsync(email);
            }
        }

        
        var existingDomainUser = await db.Users
            .FirstOrDefaultAsync(u => u.IdentityUserId == existingIdentityUser!.Id);

        if (existingDomainUser == null)
        {
            existingDomainUser = new User
            {
                IdentityUserId = existingIdentityUser.Id,
                FirstName = "Test",
                LastName = "User"
            };

            db.Users.Add(existingDomainUser);
            await db.SaveChangesAsync();
        }

        
        if (!await db.Bookings.AnyAsync())
        {
            await db.Bookings.AddAsync(new Booking
            {
                StartDate = DateTime.Today.AddHours(10),
                EndDate = DateTime.Today.AddHours(12),
                UserId = existingDomainUser.Id,
                RoomId = roomId
            });

            await db.SaveChangesAsync();
        }
    }
}
