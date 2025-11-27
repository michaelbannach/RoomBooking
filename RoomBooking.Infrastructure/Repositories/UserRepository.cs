using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Models;
using RoomBooking.Infrastructure.Data;

namespace RoomBooking.Infrastructure.Repositories;

public class UserRepository  : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public Task<List<User>> GetAllUsersAsync() =>
    _context.Set<User>().ToListAsync();
    
    public Task<User?> GetUserByIdAsync(int id) =>
    _context.Set<User>().FirstOrDefaultAsync(u => u.Id == id);
    
    public Task<User?> GetByIdentityUserIdAsync(string identityUserId)
        => _context.Set<User>().FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);

    public async Task<bool> CreateUserAsync(User user)
    {
        _context.Set<User>().Add(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        _context.Set<User>().Update(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteUserAsync(User user)
    {
        _context.Set<User>().Remove(user);
        return await _context.SaveChangesAsync() > 0;
    }
}