using System.Threading.Tasks;
using Moq;
using Xunit;

namespace RoomBooking.Application.Tests.Services.RoomService;

public class RoomService_DeleteRoomTests : RoomServiceTestBase
{
    [Fact]
    public async Task DeleteRoomAsync_ReturnsError_WhenIdLessOrEqualZero()
    {
        var room = CreateValidRoom();
        room.Id = 0;

        var (deleted, error) = await Sut.DeleteRoomAsync(room);

        Assert.False(deleted);
        Assert.Equal("Ungültige Room ID", error);
        RoomRepoMock.Verify(r => r.DeleteRoomByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteRoomAsync_ReturnsError_WhenRepositoryDeleteFails()
    {
        var room = CreateValidRoom();

        RoomRepoMock.Setup(r => r.DeleteRoomByIdAsync(room.Id))
            .ReturnsAsync(false);

        var (deleted, error) = await Sut.DeleteRoomAsync(room);

        Assert.False(deleted);
        Assert.Equal("Fehler beim Löschen des Raums", error);
    }

    [Fact]
    public async Task DeleteRoomAsync_ReturnsTrue_WhenValid()
    {
        var room = CreateValidRoom();

        RoomRepoMock.Setup(r => r.DeleteRoomByIdAsync(room.Id))
            .ReturnsAsync(true);

        var (deleted, error) = await Sut.DeleteRoomAsync(room);

        Assert.True(deleted);
        Assert.Null(error);
        RoomRepoMock.Verify(r => r.DeleteRoomByIdAsync(room.Id), Times.Once);
    }
}