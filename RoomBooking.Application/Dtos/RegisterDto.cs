using System.ComponentModel.DataAnnotations;

namespace RoomBooking.Application.Dtos;

public record RegisterDto
{
    [Required]
    [MaxLength(50)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required] public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
}