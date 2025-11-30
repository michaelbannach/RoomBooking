
using RoomBooking.Domain.Models;

namespace RoomBooking.Application.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetByIdentityUserIdAsync(string identityUserId);
    Task<(bool success, string? error, User? user)> CreateUserAsync(
        string identityUserId,
        string firstName,
        string lastName);
    
    Task<(bool success, string? error)> UpdateUserAsync(User user);
    Task<(bool success, string? error)> DeleteUserAsync(User user);
}