using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomBookingC.Data;
using RoomBookingC.Models;

namespace RoomBookingC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly RoomBookContext _context;

    public RoomController(RoomBookContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<Room>> GetAll()
    {
        return await _context.Rooms.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetById(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        return room == null ? NotFound() : Ok(room);
    }
}
