using System.ComponentModel.DataAnnotations;


namespace RoomBooking.Domain.Models;

public class User 
{
    public int Id { get; set; }
    
    [Required]
    public string IdentityUserId { get; set; } = null!;
   
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
}