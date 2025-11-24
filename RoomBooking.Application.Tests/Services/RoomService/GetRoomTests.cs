using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace RoomBooking.Application.Tests.Services.RoomService;

public class RoomService_GetRoomTests : RoomServiceTestBase
{
    [Fact]
    public async Task GetRoomByIdAsync_ThrowsArgumentException_WhenIdLessOrEqualZero()
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => Sut.GetRoomByIdAsync(0));
        Assert.Equal("Room Id must be greater than zero", ex.Message);
    }

    [Fact]
    public async Task GetRoomByIdAsync_CallsRepositoryWithCorrectId()
    {
        var room = CreateValidRoom();

        RoomRepoMock.Setup(r => r.GetRoomByIdAsync(room.Id))
            .ReturnsAsync(room);

        var result = await Sut.GetRoomByIdAsync(room.Id);

        Assert.Equal(room, result);
        RoomRepoMock.Verify(r => r.GetRoomByIdAsync(room.Id), Times.Once);
    }
}