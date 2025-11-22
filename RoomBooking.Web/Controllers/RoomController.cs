using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Models;

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
    public async Task<ActionResult<List<Room>>> GetAllRooms()
    {
        var result = await _roomService.GetAllRoomsAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetRoomById(int id)
    {
        var result = await _roomService.GetRoomByIdAsync(id);
        if(result == null)
            return NotFound();
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult> AddRoom([FromBody] Room room)
    {
        var (success, error) = await _roomService.AddRoomAsync(room);
        if (!success)
            return BadRequest(error);
        return Ok(room);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateRoom(int id, [FromBody] Room room)
    {
        if (id != room.Id)
            return BadRequest("Room ID mismatch");

        var (success, error) = await _roomService.UpdateRoomAsync(room);
        if (!success)
            return BadRequest(error);
        return Ok(room);
    }

    [HttpDelete("{id}")]
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