namespace RoomBookingC.Models;

public class Booking
{
    public int Id { get; set; }
    
    public int  RoomId { get; set; }
    
    public int EmployeeId { get; set; }
    
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    
    
}