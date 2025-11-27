using RoomBooking.Application.Dtos;

namespace RoomBooking.Application.Interfaces;

public interface IAuthService
{
    Task<(bool success, string? error)> LoginAsync(LoginDto dto);

    Task<(bool success, string? error, string? appUserId, int? userId)> RegisterAsync(RegisterDto dto);

    Task LogoutAsync();
}