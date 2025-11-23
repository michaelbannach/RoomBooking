using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RoomBooking.Domain.Models;

public class Employee : IdentityUser
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    
   
}