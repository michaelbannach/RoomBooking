using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Models;
using RoomBooking.Infrastructure.Data;


namespace RoomBooking.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _context;

    public RoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Room>> GetAllRoomsAsync()
    {
        return await _context.Rooms.ToListAsync();
    }

    public async Task<Room?> GetRoomByIdAsync(int roomId)
    {
        return await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
    }

    public async Task<bool> AddRoomAsync(Room room)
    {
        await _context.Rooms.AddAsync(room);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateRoomAsync(Room room)
    {
        var existing = await _context.Rooms.FindAsync(room.Id);
        if (existing == null)
            return false;

        existing.Name = room.Name;
        existing.Capacity = room.Capacity;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRoomByIdAsync(int roomId)
    {
        var room = await _context.Rooms.FindAsync(roomId);
        if (room == null) return false;
        _context.Rooms.Remove(room);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RoomExistsAsync(string name, int excludeId)

    {
        return await _context.Rooms
            .AnyAsync(r => r.Name == name && r.Id != excludeId);

    }
}