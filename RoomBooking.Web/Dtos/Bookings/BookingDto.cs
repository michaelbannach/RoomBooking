namespace RoomBooking.Web.Dtos.Bookings;

public record BookingDto(

    int Id,
    DateTime StartDate,
    DateTime EndDate,
    int UserId,
    int RoomId);
