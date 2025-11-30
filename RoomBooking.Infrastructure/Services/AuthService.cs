using Microsoft.AspNetCore.Identity;

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

    public AuthService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IUserService userService,
        AppDbContext dbContext,
        ILogger<AuthService> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userService = userService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<(bool success, string? error)> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("LoginAsync: Benutzer mit E-Mail {Email} nicht gefunden", email);
            return (false, "Ungültige Anmeldedaten.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("LoginAsync: Passwort falsch für Benutzer {Email}", email);
            return (false, "Ungültige Anmeldedaten.");
        }

       
        return (true, null);
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
            _logger.LogWarning("RegisterAsync: Identity-User konnte nicht erstellt werden: {Errors}", errorText);
            await transaction.RollbackAsync();
            return (false, errorText, null, null);
        }

        var (success, error, user) = await _userService.CreateUserAsync(
            appUser.Id,
            firstName,
            lastName);

        if (!success || user == null)
        {
            _logger.LogError("RegisterAsync: Domain-User konnte nicht erstellt werden: {Error}", error);
            await _userManager.DeleteAsync(appUser);
            await transaction.RollbackAsync();
            return (false, error ?? "Fehler beim Erstellen des Users.", null, null);
        }

        await transaction.CommitAsync();

        return (true, null, appUser.Id, user.Id);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
