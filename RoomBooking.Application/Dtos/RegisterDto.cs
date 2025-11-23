using System.ComponentModel.DataAnnotations;

namespace RoomBooking.Application.Dtos;

public record RegisterDto
{
    [Required]
    [MaxLength(50)]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
}