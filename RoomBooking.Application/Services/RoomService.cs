using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Models;

namespace RoomBooking.Application.Services;

public class RoomService  : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly ILogger<RoomService> _logger;

    public RoomService(IRoomRepository roomRepository, ILogger<RoomService> logger)
    {
        _roomRepository = roomRepository;
        _logger = logger;
    }

    public async Task<List<Room>> GetAllRoomsAsync()
    {
        return await _roomRepository.GetAllRoomsAsync();
    }

    public async Task<Room?> GetRoomByIdAsync(int roomId)
    {
        if (roomId <= 0)
        {
            _logger.LogWarning("GetRoomByIdAsync: Invalid RoomId {RoomId}", roomId);
            throw new ArgumentException("Room Id must be greater than zero. Not Allowd");
        }
        return await _roomRepository.GetRoomByIdAsync(roomId);
    }


}