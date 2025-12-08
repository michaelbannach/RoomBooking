using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using RoomBooking.Application.Interfaces;
using RoomBooking.Infrastructure.Data;
using RoomBooking.Infrastructure.Models;

namespace RoomBooking.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserService _userService;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _configuration;

    public AuthService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IUserService userService,
        AppDbContext dbContext,
        ILogger<AuthService> logger,
        IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userService = userService;
        _dbContext = dbContext;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<(bool success, string? error, string? token)> LoginAsync(string email, string password)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null)
        {
            _logger.LogWarning("LoginAsync: User {Email} not found", email);
            return (false, "User not found", null);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(appUser, password, false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("LoginAsync: Incorrect password{Email}", email);
            return (false, "Incorrect password", null);
        }
        
        var domainUser = await _userService.GetByIdentityUserIdAsync(appUser.Id);
        
        var token = GenerateJwtToken(appUser, domainUser);
        
        return (true, null, token);
    }

    public async Task<(bool success, string? error, string? appUserId, int? userId)> RegisterAsync(string email, string password, string firstName, string lastName)
    {
        var appUser = new ApplicationUser
        {
            UserName = email,
            Email = email
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        var identityResult = await _userManager.CreateAsync(appUser, password);
        if (!identityResult.Succeeded)
        {
            var errorText = string.Join("; ", identityResult.Errors.Select(e => e.Description));
            _logger.LogWarning("RegisterAsync: Failed to create Identity-User: {Errors}", errorText);
            await transaction.RollbackAsync();
            return (false, errorText, null, null);
        }

        var (success, error, user) = await _userService.CreateUserAsync(
            appUser.Id,
            firstName,
            lastName);

        if (!success || user == null)
        {
            _logger.LogError("RegisterAsync: Failed to create domain user: {Error}", error);
            await _userManager.DeleteAsync(appUser);
            await transaction.RollbackAsync();
            return (false, error ?? "Failed to create domain user", null, null);
        }

        await transaction.CommitAsync();

        return (true, null, appUser.Id, user.Id);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
    
    private string GenerateJwtToken(ApplicationUser appUser, Domain.Models.User? domainUser)
    {
        var jwtSection = _configuration.GetSection("JWT");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, appUser.Id),
            new(JwtRegisteredClaimNames.Email, appUser.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, appUser.Id)
        };

        if (domainUser != null)
        {
            // eigener Claim „userId“ = Domain-User-Id (für Bookings wichtig)
            claims.Add(new Claim("userId", domainUser.Id.ToString()));
            claims.Add(new Claim("firstName", domainUser.FirstName));
            claims.Add(new Claim("lastName", domainUser.LastName));
        }

        var expiresMinutes = int.TryParse(jwtSection["ExpiresMinutes"], out var m) ? m : 60;

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
