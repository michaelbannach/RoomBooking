using RoomBooking.Domain.Models;
using RoomBooking.Web.Dtos.Users;

namespace RoomBooking.Web.Mappings;

public static class UserMappings
{
    public static UserDto ToDto(this User user) =>
        new(user.Id, user.FirstName, user.LastName);
    
    public static IEnumerable<UserDto> ToDtos(this IEnumerable<User> users) =>
        users.Select(u => u.ToDto());
}