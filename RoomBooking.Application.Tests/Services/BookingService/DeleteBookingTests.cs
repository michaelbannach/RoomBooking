using Moq;

namespace RoomBooking.Application.Tests.Services.BookingService;

public class BookingService_DeleteBookingTests : BookingServiceTestBase
{
    [Fact]
    public async Task DeleteBookingAsync_ReturnsError_WhenBookingIsNull()
    {
        var (deleted, error) = await Sut.DeleteBookingAsync(null!);

        Assert.False(deleted);
        Assert.Equal("Booking must not be null", error);
        RepoMock.Verify(r => r.DeleteBookingByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteBookingAsync_ReturnsError_WhenIdLessOrEqualZero()
    {
        var booking = CreateValidBooking();
        booking.Id = 0;

        var (deleted, error) = await Sut.DeleteBookingAsync(booking);

        Assert.False(deleted);
        Assert.Equal("Invalid BookingId", error);
        RepoMock.Verify(r => r.DeleteBookingByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteBookingAsync_ReturnsError_WhenUserIdIsEmpty()
    {
        var booking = CreateValidBooking();
        booking.UserId = 0;

        var (deleted, error) = await Sut.DeleteBookingAsync(booking);

        Assert.False(deleted);
        Assert.Equal("Empty UserId", error);
        RepoMock.Verify(r => r.DeleteBookingByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteBookingAsync_ReturnsError_WhenRepositoryDeleteFails()
    {
        var booking = CreateValidBooking();

        RepoMock
            .Setup(r => r.DeleteBookingByIdAsync(booking.Id))
            .ReturnsAsync(false);

        var (deleted, error) = await Sut.DeleteBookingAsync(booking);

        Assert.False(deleted);
        Assert.Equal("Error deleting booking", error);
    }

    [Fact]
    public async Task DeleteBookingAsync_ReturnsTrue_WhenValid()
    {
        var booking = CreateValidBooking();

        RepoMock
            .Setup(r => r.DeleteBookingByIdAsync(booking.Id))
            .ReturnsAsync(true);

        var (deleted, error) = await Sut.DeleteBookingAsync(booking);

        Assert.True(deleted);
        Assert.Null(error);
        RepoMock.Verify(r => r.DeleteBookingByIdAsync(booking.Id), Times.Once);
    }
}
