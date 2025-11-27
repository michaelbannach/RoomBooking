using Moq;
using RoomBooking.Domain.Models;


namespace RoomBooking.Application.Tests.Services.BookingService;

public class BookingService_UpdateBookingTests : BookingServiceTestBase
{
    [Fact]
    public async Task UpdateBookingAsync_ReturnsError_WhenBookingIsNull()
    {
        var (updated, error) = await Sut.UpdateBookingAsync(null!);

        Assert.False(updated);
        Assert.Equal("Booking darf nicht null sein.", error);
        RepoMock.Verify(r => r.UpdateBookingAsync(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public async Task UpdateBookingAsync_ReturnsError_WhenIdLessOrEqualZero()
    {
        var booking = CreateValidBooking();
        booking.Id = 0;

        var (updated, error) = await Sut.UpdateBookingAsync(booking);

        Assert.False(updated);
        Assert.Equal("Ungültige Booking-Id.", error);
        RepoMock.Verify(r => r.GetBookingByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateBookingAsync_ReturnsError_WhenBookingNotFound()
    {
        var booking = CreateValidBooking();

        RepoMock
            .Setup(r => r.GetBookingByIdAsync(booking.Id))
            .ReturnsAsync((Booking?)null);

        var (updated, error) = await Sut.UpdateBookingAsync(booking);

        Assert.False(updated);
        Assert.Equal($"Booking mit Id {booking.Id} nicht gefunden.", error);
        RepoMock.Verify(r => r.UpdateBookingAsync(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public async Task UpdateBookingAsync_ReturnsError_WhenStartDateNotBeforeEndDate()
    {
        var booking = CreateValidBooking();
        booking.StartDate = DateTime.Now.AddHours(1);
        booking.EndDate = booking.StartDate;

        RepoMock
            .Setup(r => r.GetBookingByIdAsync(booking.Id))
            .ReturnsAsync(new Booking { Id = booking.Id });

        var (updated, error) = await Sut.UpdateBookingAsync(booking);

        Assert.False(updated);
        Assert.Equal("Startzeit muss vor Endzeit liegen.", error);
        RepoMock.Verify(r => r.UpdateBookingAsync(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public async Task UpdateBookingAsync_ReturnsError_WhenStartDateInPast()
    {
        var booking = CreateValidBooking();
        booking.StartDate = DateTime.Now.AddHours(-1);
        booking.EndDate = DateTime.Now.AddHours(1);

        RepoMock
            .Setup(r => r.GetBookingByIdAsync(booking.Id))
            .ReturnsAsync(new Booking { Id = booking.Id });

        var (updated, error) = await Sut.UpdateBookingAsync(booking);

        Assert.False(updated);
        Assert.Equal("StartTime in der Vergangenheit", error);
        RepoMock.Verify(r => r.UpdateBookingAsync(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public async Task UpdateBookingAsync_ReturnsError_WhenOverlappingBookingExists()
    {
        var booking = CreateValidBooking();
        booking.StartDate = DateTime.Now.AddHours(1);
        booking.EndDate = DateTime.Now.AddHours(2);

        var overlapping = new Booking
        {
            Id = 99,
            RoomId = booking.RoomId,
            StartDate = booking.StartDate.AddMinutes(-30),
            EndDate = booking.StartDate.AddMinutes(30)
        };

        RepoMock
            .Setup(r => r.GetBookingByIdAsync(booking.Id))
            .ReturnsAsync(new Booking { Id = booking.Id });

        RepoMock
            .Setup(r => r.GetBookingsByRoomIdAsync(booking.RoomId))
            .ReturnsAsync(new List<Booking> { overlapping });

        var (updated, error) = await Sut.UpdateBookingAsync(booking);

        Assert.False(updated);
        Assert.Equal("Raum ist im gewünschten Zeitraum bereits gebucht.", error);
        RepoMock.Verify(r => r.UpdateBookingAsync(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public async Task UpdateBookingAsync_ReturnsError_WhenRepositoryUpdateFails()
    {
        var booking = CreateValidBooking();
        booking.StartDate = DateTime.Now.AddHours(1);
        booking.EndDate = DateTime.Now.AddHours(2);

        RepoMock
            .Setup(r => r.GetBookingByIdAsync(booking.Id))
            .ReturnsAsync(new Booking { Id = booking.Id });

        RepoMock
            .Setup(r => r.GetBookingsByRoomIdAsync(booking.RoomId))
            .ReturnsAsync(new List<Booking>());

        RepoMock
            .Setup(r => r.UpdateBookingAsync(booking))
            .ReturnsAsync(false);

        var (updated, error) = await Sut.UpdateBookingAsync(booking);

        Assert.False(updated);
        Assert.Equal("Fehler beim Aktualisieren der Buchung.", error);
    }

    [Fact]
    public async Task UpdateBookingAsync_ReturnsTrue_WhenValid()
    {
        var booking = CreateValidBooking();
        booking.StartDate = DateTime.Now.AddHours(1);
        booking.EndDate = DateTime.Now.AddHours(2);

        RepoMock
            .Setup(r => r.GetBookingByIdAsync(booking.Id))
            .ReturnsAsync(new Booking { Id = booking.Id });

        RepoMock
            .Setup(r => r.GetBookingsByRoomIdAsync(booking.RoomId))
            .ReturnsAsync(new List<Booking>());

        RepoMock
            .Setup(r => r.UpdateBookingAsync(booking))
            .ReturnsAsync(true);

        var (updated, error) = await Sut.UpdateBookingAsync(booking);

        Assert.True(updated);
        Assert.Null(error);
        RepoMock.Verify(r => r.UpdateBookingAsync(booking), Times.Once);
    }
}
