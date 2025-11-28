using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Models;
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
    
    [HttpPost]
    public async Task<ActionResult<RoomDto>> AddRoom([FromBody] RoomCreateDto dto)
    {
        var room = dto.ToEntity();
        
        var (success, error) = await _roomService.AddRoomAsync(room);
        if (!success)
            return BadRequest(error);
        return Ok(room.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RoomDto>> UpdateRoom(int id, [FromBody] RoomUpdateDto dto)
    {
        
        if (id != dto.Id)
            return BadRequest("Room ID mismatch");

        var existing = await _roomService.GetRoomByIdAsync(id);
        if (existing == null)
            return NotFound();
        
        var (success, error) = await _roomService.UpdateRoomAsync(existing);
        if (!success)
            return BadRequest(error);
        return Ok(existing.ToDto());
        
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteRoom(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { error = "Ungültige Id" });
        }
        var room = await _roomService.GetRoomByIdAsync(id);
        if(room == null)
            return NotFound();
        
        var (deleted, error) = await _roomService.DeleteRoomAsync(room);
        if (!deleted)
            return BadRequest(error);

        return Ok();
    }

}