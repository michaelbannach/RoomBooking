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

    public async Task<(bool added, string? error)> AddRoomAsync(Room room)
    {
        if (room == null)
        {
            _logger.LogWarning("AddRoomAsync: Room is null");
            return(false, "Room is null");
        }
        
        var exists = await _roomRepository.RoomExistsAsync(room.Name, room.Id);
        if (exists)
        {
            _logger.LogWarning("AddRoomAsync: Roomname already exists");
            return(false, "Room already exists");
        }

        if (room.Capacity <= 0)
        {
            _logger.LogWarning("AddRoomAsync: Invalid Capacity. Must be greater than zero. Not Allowed");
            return(false, "Capacity must not be equal or less than zero");
        }
        
        var ok = await _roomRepository.AddRoomAsync(room);
        if (!ok)
        {
            _logger.LogError("AddRoomAsync: Error while adding new room");
            return(false, "Error while adding new room");
        }

        return (true, null);
     
     
    }

    public async Task<(bool updated, string? error)> UpdateRoomAsync(Room room)
    {
        if (room == null)
        {
            _logger.LogWarning("UpdateRoomAsync: Room is null");
            return (false, "Room must not be null");
        }
        
        if (room.Id <= 0)
        {
            _logger.LogWarning("UpdateRoomAsync: Invalid RoomId {RoomId}", room.Id);
            return (false, "Invalid RoomId");
        }

        if (room.Capacity <= 0)
        {
            _logger.LogWarning("AddRoomAsync: Invalid Capacity. Must be greater than zero. Not Allowed");
            return(false, "Capacity must not be equal or less than zero");
        }
        
        var exists = await _roomRepository.RoomExistsAsync(room.Name, room.Id);
        if (exists)
        {
            _logger.LogWarning("UpdateRoomAsync: Roomname already exists");
            return(false, "Roomname already exists");
        }
       
        var result = await _roomRepository.UpdateRoomAsync(room);
        if (!result)
        {
            _logger.LogError("UpdateRoomAsync: Error while update RoomId {RoomId}", room.Id);
            return (false, "Error while updating room");
        }
         
        return (true, null);
    }

    public async Task<(bool deleted, string? error)> DeleteRoomAsync(Room room)
    {
        if (room.Id <= 0)
        {
            _logger.LogWarning("DeleteRoomByIdAsync: Invalid RoomId {RoomId}", room.Id);
            return (false, "Invalid RoomId");
        }

        var deleted = await _roomRepository.DeleteRoomByIdAsync(room.Id);
        if (!deleted)
        {
            _logger.LogError("DeleteRoomByIdAsync: Error while deleting RoomId {RoomId}", room.Id);
            return (false, "Error while deleting room");
        }
        return (true, null);
    }
    
    
}