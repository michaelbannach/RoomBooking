using RoomBooking.Domain.Models;
using RoomBooking.Web.Dtos.Rooms;

namespace RoomBooking.Web.Mappings;

public static class RoomMappings
{
    public static RoomDto ToDto(this Room room) =>
        new(room.Id, room.Name, room.Capacity);
    
    public static IEnumerable<RoomDto> ToDto(this IEnumerable<Room> rooms) =>
        rooms.Select(r => r.ToDto());
    
}