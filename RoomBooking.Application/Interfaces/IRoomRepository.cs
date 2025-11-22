using RoomBooking.Domain.Models;


namespace RoomBooking.Application.Interfaces;

public interface IRoomRepository
{
    Task<List<Room>> GetAllRoomsAsync();
    
    Task<Room?> GetRoomByIdAsync(int roomId);
    
    Task<bool> CreateRoomAsync(Room room);
    
    Task<bool> UpdateRoomAsync(Room room);
    
    Task<bool> DeleteRoomByIdAsync(int roomId);
}