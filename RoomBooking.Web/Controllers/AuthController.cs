using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.Dtos;
using RoomBooking.Application.Interfaces;

namespace RoomBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _authService.LoginAsync(dto);
        if (!success)
            return Unauthorized(new { error });

        // später: JWT im Response zurückgeben
        return Ok(new { Message = "Login ok" });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var (success, error, appUserId, userId) = await _authService.RegisterAsync(dto);
        if (!success)
            return BadRequest(new { error });

        return Ok(new
        {
            appUserId,
            userId,
            email = dto.Email
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return Ok(new { Message = "Logout ok" });
    }
}