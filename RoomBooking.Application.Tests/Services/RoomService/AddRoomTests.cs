using System.Threading.Tasks;
using Moq;
using RoomBooking.Domain.Models;
using Xunit;

namespace RoomBooking.Application.Tests.Services.RoomService;

public class RoomService_AddRoomTests : RoomServiceTestBase
{
    [Fact]
    public async Task AddRoomAsync_ReturnsError_WhenRoomIsNull()
    {
        var (added, error) = await Sut.AddRoomAsync(null!);

        Assert.False(added);
        Assert.Equal("Room ist NULL", error);
        RoomRepoMock.Verify(r => r.AddRoomAsync(It.IsAny<Room>()), Times.Never);
    }

    [Fact]
    public async Task AddRoomAsync_ReturnsError_WhenRoomNameAlreadyExists()
    {
        var room = CreateValidRoom();

        RoomRepoMock.Setup(r => r.RoomExistsAsync(room.Name, room.Id))
            .ReturnsAsync(true);

        var (added, error) = await Sut.AddRoomAsync(room);

        Assert.False(added);
        Assert.Equal("Raumname ist bereits vergeben", error);
        RoomRepoMock.Verify(r => r.AddRoomAsync(It.IsAny<Room>()), Times.Never);
    }

    [Fact]
    public async Task AddRoomAsync_ReturnsError_WhenCapacityIsLessOrEqualZero()
    {
        var room = CreateValidRoom();
        room.Capacity = 0;

        RoomRepoMock.Setup(r => r.RoomExistsAsync(room.Name, room.Id))
            .ReturnsAsync(false);

        var (added, error) = await Sut.AddRoomAsync(room);

        Assert.False(added);
        Assert.Equal("Capacity darf nicht kleiner gleich null sein", error);
        RoomRepoMock.Verify(r => r.AddRoomAsync(It.IsAny<Room>()), Times.Never);
    }

    [Fact]
    public async Task AddRoomAsync_ReturnsError_WhenRepositorySaveFails()
    {
        var room = CreateValidRoom();

        RoomRepoMock.Setup(r => r.RoomExistsAsync(room.Name, room.Id))
            .ReturnsAsync(false);

        RoomRepoMock.Setup(r => r.AddRoomAsync(room))
            .ReturnsAsync(false);

        var (added, error) = await Sut.AddRoomAsync(room);

        Assert.False(added);
        Assert.Equal("Fehler beim Speichern", error);
    }

    [Fact]
    public async Task AddRoomAsync_ReturnsTrue_WhenValid()
    {
        var room = CreateValidRoom();

        RoomRepoMock.Setup(r => r.RoomExistsAsync(room.Name, room.Id))
            .ReturnsAsync(false);

        RoomRepoMock.Setup(r => r.AddRoomAsync(room))
            .ReturnsAsync(true);

        var (added, error) = await Sut.AddRoomAsync(room);

        Assert.True(added);
        Assert.Null(error);
        RoomRepoMock.Verify(r => r.AddRoomAsync(room), Times.Once);
    }
}
