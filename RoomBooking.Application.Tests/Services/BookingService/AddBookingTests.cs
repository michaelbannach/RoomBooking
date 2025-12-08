using Moq;
using RoomBooking.Domain.Models;


namespace RoomBooking.Application.Tests.Services.BookingService;

public class BookingService_AddBookingTests : BookingServiceTestBase
{
    [Fact]
    public async Task AddBookingAsync_ReturnsError_WhenUserIdIsEmpty()
    {
        var booking = CreateValidBooking();

        var (added, error) = await Sut.AddBookingAsync(0, booking);

        Assert.False(added);
        Assert.Equal("Unknown user", error);
        RepoMock.Verify(r => r.AddBookingAsync(It.IsAny<Booking>()), Times.Never);
    }

    
    [Fact]
    public async Task AddBookingAsync_ReturnsError_WhenStartDateInPast()
    {
        var booking = CreateValidBooking();
        booking.StartDate = DateTime.Now.AddHours(-2);

        var (added, error) = await Sut.AddBookingAsync(1, booking);

        Assert.False(added);
        Assert.Equal("StartTime in the past. Not Allowed", error);
        RepoMock.Verify(r => r.AddBookingAsync(It.IsAny<Booking>()), Times.Never);
    }

    
    [Fact]
    public async Task AddBookingAsync_ReturnsError_WhenEndDateBeforeOrEqualStartDate()
    {
        var booking = CreateValidBooking();
        booking.EndDate = booking.StartDate;

        var (added, error) = await Sut.AddBookingAsync(1, booking);

        Assert.False(added);
        Assert.Equal("EndTime earlier or equal StartTime", error);
        RepoMock.Verify(r => r.AddBookingAsync(It.IsAny<Booking>()), Times.Never);
    }
    
    [Fact]
    public async Task AddBookingAsync_ReturnsError_WhenSavingToRepositoryFails()
    {
        var booking = CreateValidBooking();

        RepoMock
            .Setup(r => r.GetBookingsByRoomIdAsync(booking.RoomId))
            .ReturnsAsync(new List<Booking>());

        RepoMock
            .Setup(r => r.AddBookingAsync(booking))
            .ReturnsAsync(false);

        var (added, error) = await Sut.AddBookingAsync(1, booking);

        Assert.False(added);
        Assert.Equal("Error while saving", error);
    }
    
    [Fact]
    public async Task AddBookingAsync_ReturnsError_WhenOverlappingBookingExists()
    {
        var booking = CreateValidBooking();

        var existingBooking = new Booking
        {
            Id = 99,
            UserId = 123,
            RoomId = booking.RoomId,
            StartDate = booking.StartDate.AddMinutes(-30),
            EndDate = booking.StartDate.AddMinutes(30)
        };

        RepoMock.Setup(r => r.GetBookingsByRoomIdAsync(booking.RoomId))
                .ReturnsAsync(new List<Booking> { existingBooking });

        var (added, error) = await Sut.AddBookingAsync(1, booking);

        Assert.False(added);
        Assert.Equal("Already booked, Overlapping", error);
        RepoMock.Verify(r => r.AddBookingAsync(It.IsAny<Booking>()), Times.Never);
    }

    
    [Fact]
    public async Task AddBookingAsync_ReturnsTrue_WhenValid()
    {
        var booking = CreateValidBooking();

        RepoMock.Setup(r => r.GetBookingsByRoomIdAsync(booking.RoomId))
                .ReturnsAsync(new List<Booking>());

        RepoMock.Setup(r => r.AddBookingAsync(booking))
                .ReturnsAsync(true);

        var (added, error) = await Sut.AddBookingAsync(1, booking);

        Assert.True(added);
        Assert.Null(error);
        RepoMock.Verify(r => r.AddBookingAsync(booking), Times.Once);
    }
}
