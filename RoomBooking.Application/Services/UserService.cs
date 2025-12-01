using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Models;

namespace RoomBooking.Application.Services;

public class UserService  : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public Task<List<User>> GetAllUsersAsync() =>
    _userRepository.GetAllUsersAsync();
    
    public Task<User?> GetUserByIdAsync(int id) 
    => _userRepository.GetUserByIdAsync(id);
    
    public Task<User?> GetByIdentityUserIdAsync(string identityUserId)
    => _userRepository.GetByIdentityUserIdAsync(identityUserId);
    
    public async Task<(bool success, string? error, User? user)> CreateUserAsync(string identityUserId, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(identityUserId))
            return (false, "IdentityUserId must not be empty.", null);

        if(string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            return (false, "FirstName & LastName are required.", null);
        
        var user = new User
        {
            IdentityUserId = identityUserId,
            FirstName = firstName,
            LastName = lastName
        };

        var ok = await _userRepository.CreateUserAsync(user);
        if (!ok)
        {
            _logger.LogError("CreateUserAsync: Error while creating user");
            return (false, "Error while creating user.", null);
        }

        return (true, null, user);
    }

    public async Task<(bool success, string? error)> UpdateUserAsync(User user)
    {
        if (user.Id <= 0)
            return (false, "Invalid UserId");

        var ok = await _userRepository.UpdateUserAsync(user);
        if (!ok)
        {
            _logger.LogError("UpdateAsync: Error while updating UserId {Id}", user.Id);
            return (false, "Error while updating UserId.");
        }

        return (true, null);
    }

    public async Task<(bool success, string? error)> DeleteUserAsync(User user)
    {
        if (user.Id <= 0)
            return (false, "Invalid UserId.");

        var ok = await _userRepository.DeleteUserAsync(user);
        if (!ok)
        {
            _logger.LogError("DeleteAsync: Error while deleting UserId {Id}", user.Id);
            return (false, "Error while deleting user.");
        }

        return (true, null);
    }
}

