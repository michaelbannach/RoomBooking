namespace RoomBooking.Web.Dtos.Bookings;

public record BookingCreateDto(
    DateTime StartDate,
    DateTime EndDate,
    int RoomId);