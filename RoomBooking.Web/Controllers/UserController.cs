using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RoomBooking.Application.Interfaces;
using RoomBooking.Web.Dtos.Users;
using RoomBooking.Web.Mappings;

namespace RoomBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    
    [HttpGet]
    [AllowAnonymous]
    
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users.ToDtos().ToList());
    }

    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user.ToDto());
    }
}