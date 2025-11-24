using System.Threading.Tasks;
using Moq;
using RoomBooking.Domain.Models;
using Xunit;

namespace RoomBooking.Application.Tests.Services.RoomService;

public class RoomService_UpdateRoomTests : RoomServiceTestBase
{
    [Fact]
    public async Task UpdateRoomAsync_ReturnsError_WhenRoomIsNull()
    {
        var (updated, error) = await Sut.UpdateRoomAsync(null!);

        Assert.False(updated);
        Assert.Equal("Room darf nicht NULL sein", error);
        RoomRepoMock.Verify(r => r.UpdateRoomAsync(It.IsAny<Room>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRoomAsync_ReturnsError_WhenIdLessOrEqualZero()
    {
        var room = CreateValidRoom();
        room.Id = 0;

        var (updated, error) = await Sut.UpdateRoomAsync(room);

        Assert.False(updated);
        Assert.Equal("Ungültige Room ID", error);
        RoomRepoMock.Verify(r => r.UpdateRoomAsync(It.IsAny<Room>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRoomAsync_ReturnsError_WhenCapacityLessOrEqualZero()
    {
        var room = CreateValidRoom();
        room.Capacity = 0;

        var (updated, error) = await Sut.UpdateRoomAsync(room);

        Assert.False(updated);
        Assert.Equal("Capacity darf nicht kleiner gleich null sein", error);
        RoomRepoMock.Verify(r => r.UpdateRoomAsync(It.IsAny<Room>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRoomAsync_ReturnsError_WhenRoomNameAlreadyExists()
    {
        var room = CreateValidRoom();

        RoomRepoMock
            .Setup(r => r.RoomExistsAsync(room.Name, room.Id))
            .ReturnsAsync(true);

        var (updated, error) = await Sut.UpdateRoomAsync(room);

        Assert.False(updated);
        Assert.Equal("Raumname ist bereits vergeben", error);
        RoomRepoMock.Verify(r => r.UpdateRoomAsync(It.IsAny<Room>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRoomAsync_ReturnsError_WhenRepositoryUpdateFails()
    {
        var room = CreateValidRoom();

        RoomRepoMock
            .Setup(r => r.RoomExistsAsync(room.Name, room.Id))
            .ReturnsAsync(false);

        RoomRepoMock
            .Setup(r => r.UpdateRoomAsync(room))
            .ReturnsAsync(false);

        var (updated, error) = await Sut.UpdateRoomAsync(room);

        Assert.False(updated);
        Assert.Equal("Fehler beim Aktualisieren", error);
    }

    [Fact]
    public async Task UpdateRoomAsync_ReturnsTrue_WhenValid()
    {
        var room = CreateValidRoom();

        RoomRepoMock
            .Setup(r => r.RoomExistsAsync(room.Name, room.Id))
            .ReturnsAsync(false);

        RoomRepoMock
            .Setup(r => r.UpdateRoomAsync(room))
            .ReturnsAsync(true);

        var (updated, error) = await Sut.UpdateRoomAsync(room);

        Assert.True(updated);
        Assert.Null(error);
        RoomRepoMock.Verify(r => r.UpdateRoomAsync(room), Times.Once);
    }
}
