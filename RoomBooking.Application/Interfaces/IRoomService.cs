using RoomBooking.Domain.Models;

namespace RoomBooking.Application.Interfaces;

public interface IRoomService
{
    Task<List<Room>> GetAllRoomsAsync();

    Task<Room?> GetRoomByIdAsync(int roomId);

    Task<(bool added, string? error)> AddRoomAsync(Room room);

    Task<(bool updated, string? error)> UpdateRoomAsync(Room room);

    Task<(bool deleted, string? error)> DeleteRoomByIdAsync(int roomId);
}