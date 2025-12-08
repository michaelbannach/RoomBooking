using RoomBooking.Domain.Models;


namespace RoomBooking.Application.Interfaces;

public interface IRoomRepository
{
    Task<List<Room>> GetAllRoomsAsync();
    
    Task<Room?> GetRoomByIdAsync(int roomId);
    
}