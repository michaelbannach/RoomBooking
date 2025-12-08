using RoomBooking.Domain.Models;
using RoomBooking.Web.Dtos.Bookings;


namespace RoomBooking.Web.Mappings;

public static class BookingMappings
{
    public static BookingDto ToDto(this Booking booking) =>
        new(booking.Id, booking.StartDate, booking.EndDate,booking.UserId, booking.RoomId);

    public static IEnumerable<BookingDto> ToDto(this IEnumerable<Booking> bookings) =>
        bookings.Select(b => b.ToDto());

    public static Booking ToEntity(this BookingCreateDto dto) =>
        new()
        {
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            RoomId = dto.RoomId
        };

    public static void Apply(this BookingUpdateDto dto, Booking booking)
    {
        booking.StartDate = dto.StartDate;
        booking.EndDate = dto.EndDate;
        booking.RoomId = dto.RoomId;
    }
}

