namespace RoomBookingC.Dtos;

public class BookingDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool CanDelete { get; set; }
}