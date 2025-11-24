using System.Threading.Tasks;
using Moq;
using RoomBooking.Domain.Models;
using Xunit;

namespace RoomBooking.Application.Tests.Services.BookingService;

public class BookingService_DeleteBookingTests : BookingServiceTestBase
{
    [Fact]
    public async Task DeleteBookingAsync_ReturnsError_WhenBookingIsNull()
    {
        var (deleted, error) = await Sut.DeleteBookingAsync(null!);

        Assert.False(deleted);
        Assert.Equal("Booking darf nicht null sein.", error);
        RepoMock.Verify(r => r.DeleteBookingByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteBookingAsync_ReturnsError_WhenIdLessOrEqualZero()
    {
        var booking = CreateValidBooking();
        booking.Id = 0;

        var (deleted, error) = await Sut.DeleteBookingAsync(booking);

        Assert.False(deleted);
        Assert.Equal("Ungültige Booking-Id.", error);
        RepoMock.Verify(r => r.DeleteBookingByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteBookingAsync_ReturnsError_WhenEmployeeIdIsEmpty()
    {
        var booking = CreateValidBooking();
        booking.EmployeeId = "   ";

        var (deleted, error) = await Sut.DeleteBookingAsync(booking);

        Assert.False(deleted);
        Assert.Equal("Leere Employee ID", error);
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
        Assert.Equal("Fehler beim Löschen der Buchung.", error);
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
