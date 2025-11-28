namespace RoomBooking.Web.Dtos.Rooms;

public record RoomUpdateDto(
    int Id,
    string Name,
    int Capacity
    );