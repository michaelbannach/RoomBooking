namespace RoomBooking.Application.Interfaces;

public interface IAuthService
{
    Task<(bool success, string? error)> LoginAsync(string email, string password);

    Task<(bool success, string? error, string? appUserId, int? userId)> RegisterAsync(string email, string password, string firstName, string lastName);

    Task LogoutAsync();
}