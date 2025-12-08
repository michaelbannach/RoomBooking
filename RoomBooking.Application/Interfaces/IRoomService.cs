using RoomBooking.Domain.Models;

namespace RoomBooking.Application.Interfaces;

public interface IRoomService
{
    Task<List<Room>> GetAllRoomsAsync();

    Task<Room?> GetRoomByIdAsync(int roomId);
    
}