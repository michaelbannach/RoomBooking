using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using RoomBooking.Domain.Models;
using RoomBooking.Application.Dtos;

namespace RoomBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<Employee> _userManager;

    public UserController(UserManager<Employee> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("{employeeId}")]
    public async Task<ActionResult<Employee>> GetEmployeeById(string employeeId)
    {
        var user = await _userManager.FindByIdAsync(employeeId);
        if(user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost("register")]
    [AllowAnonymous]

    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new Employee
        {
            UserName = dto.Email,
            Email = dto.Email,
        };
        
        var result = await _userManager.CreateAsync(user, dto.Password);
        if(!result.Succeeded) 
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        
        return Ok("Employee registered");
    }

   
}

