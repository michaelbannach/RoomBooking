namespace RoomBooking.Web.Dtos.Bookings;

public record BookingUpdateDto(
    int Id,
    DateTime StartDate,
    DateTime EndDate,
    int UserId,
    int RoomId);

    
