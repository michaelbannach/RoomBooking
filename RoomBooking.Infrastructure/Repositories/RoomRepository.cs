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
    
}