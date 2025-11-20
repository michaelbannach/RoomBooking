using System.ComponentModel.DataAnnotations;

namespace RoomBooking.Domain.Models;

public class Booking
{
    public int Id { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    public string EmployeeId { get; set; }
    
    
    public Employee Employee { get; set; }
    
    [Required]
   public int RoomId { get; set; }
    
    public Room Room { get; set; }
}