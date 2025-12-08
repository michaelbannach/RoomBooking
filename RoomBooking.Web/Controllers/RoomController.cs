using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.Interfaces;
using RoomBooking.Web.Dtos.Rooms;
using RoomBooking.Web.Mappings;

namespace RoomBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoomDto>>> GetAllRooms()
    {
        var rooms = await _roomService.GetAllRoomsAsync();
        return Ok(rooms.ToDto().ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoomDto>> GetRoomById(int id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        if(room == null)
            return NotFound();
        return Ok(room.ToDto());
    }
    
  

}