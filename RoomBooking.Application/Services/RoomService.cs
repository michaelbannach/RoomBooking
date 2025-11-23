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
            _logger.LogError("GetRoomByIdAsync: Ungültige Room ID {RoomId}", roomId);
            throw new ArgumentException("Room Id must be greater than zero");
        }
        return await _roomRepository.GetRoomByIdAsync(roomId);
    }

    public async Task<(bool added, string? error)> AddRoomAsync(Room room)
    {
        if (room == null)
        {
            _logger.LogWarning("AddRoomAsync: Room ist NULL");
            return(false, "Room ist NULL");
        }
        
        var exists = await _roomRepository.RoomExistsAsync(room.Name);
        if (exists)
        {
            _logger.LogError("AddRoomAsync: Raumname bereits vergeben");
            return(false, "Raumname ist bereits vergeben");
        }

        if (room.Capacity <= 0)
        {
            _logger.LogError("AddRoomAsync: Capacity darf nicht kleiner gleich null sein");
            return(false, "Capacity darf nicht kleiner gleich null sein");
        }
        
        var ok = await _roomRepository.AddRoomAsync(room);
        if (!ok)
        {
            _logger.LogError("AddRoomAsync: Fehler beim Speichern");
            return(false, "Fehler beim Speichern");
        }

        return (true, null);
     
     
    }

    public async Task<(bool updated, string? error)> UpdateRoomAsync(Room room)
    {
        if (room == null)
        {
            _logger.LogError("UpdateRoomAsync: Room ist NULL");
            return (false, "Room darf nicht NULL sein");
        }
        
        if (room.Id <= 0)
        {
            _logger.LogWarning("UpdateRoomAsync: Ungültige Room ID {RoomId}", room.Id);
            return (false, "Ungültige Room ID");
        }

        if (room.Capacity <= 0)
        {
            _logger.LogError("AddRoomAsync: Capacity darf nicht kleiner gleich null sein");
            return(false, "Capacity darf nicht kleiner gleich null sein");
        }
        
        var exists = await _roomRepository.RoomExistsAsync(room.Name);
        if (exists)
        {
            _logger.LogError("UpdateRoomAsync: Raumname bereits vergeben");
            return(false, "Raumname ist bereits vergeben");
        }
       
        var result = await _roomRepository.UpdateRoomAsync(room);
        if (!result)
        {
            _logger.LogError("UpdateRoomAsync: Fehler beim Aktualisieren des Raums mit Id {RoomId}", room.Id);
            return (false, "Fehler beim Aktualisieren");
        }
         
        return (true, null);
    }

    public async Task<(bool deleted, string? error)> DeleteRoomAsync(Room room)
    {
        if (room.Id <= 0)
        {
            _logger.LogWarning("DeleteRoomByIdAsync: Ungültige Room ID {RoomId}", room.Id);
            return (false, "Ungültige Room ID");
        }

        var deleted = await _roomRepository.DeleteRoomByIdAsync(room.Id);
        if (!deleted)
        {
            _logger.LogError("DeleteRoomByIdAsync: Fehler beim Löschen des Raums mit der Id {RoomId}", room.Id);
            return (false, "Fehler beim Löschen des Raums");
        }
        return (true, null);
    }
    
    
}