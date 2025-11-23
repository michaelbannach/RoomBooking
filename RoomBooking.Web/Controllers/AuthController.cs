using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using RoomBooking.Domain.Models;
using RoomBooking.Application.Dtos;

namespace RoomBooking.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController  : ControllerBase
{
    private readonly SignInManager<Employee> _signInManager;
    private readonly UserManager<Employee> _userManager;

    public AuthController(
        SignInManager<Employee> signInManager,
        UserManager<Employee> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if(user == null)
            return Unauthorized("Ungültige Email oder  Passwort");
        
        var result = await _signInManager.PasswordSignInAsync(user, dto.Password, isPersistent: true, false);
        
        if(!result.Succeeded)
            return Unauthorized("Ungültige Email oder  Passwort");
        
        return Ok("Logged In");
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok("Logged Out");
    }
}





