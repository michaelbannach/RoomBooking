using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using RoomBooking.Domain.Models;
using Xunit;

namespace RoomBooking.Application.Tests.Services.BookingService;

public class BookingService_AddBookingTests : BookingServiceTestBase
{
    [Fact]
    public async Task AddBookingAsync_ReturnsError_WhenEmployeeIdIsEmpty()
    {
        var booking = CreateValidBooking();

        var (added, error) = await Sut.AddBookingAsync("", booking);

        Assert.False(added);
        Assert.Equal("Unbekannter Benutzer", error);
        RepoMock.Verify(r => r.AddBookingAsync(It.IsAny<Booking>()), Times.Never);
    }

    // StartDate in Vergangenheit --> Fehler
    [Fact]
    public async Task AddBookingAsync_ReturnsError_WhenStartDateInPast()
    {
        var booking = CreateValidBooking();
        booking.StartDate = DateTime.Now.AddHours(-2);

        var (added, error) = await Sut.AddBookingAsync("emp-1", booking);

        Assert.False(added);
        Assert.Equal("StartTime in der Vergangenheit", error);
        RepoMock.Verify(r => r.AddBookingAsync(It.IsAny<Booking>()), Times.Never);
    }

    // EndDate <= StartDate --> Fehler
    [Fact]
    public async Task AddBookingAsync_ReturnsError_WhenEndDateBeforeOrEqualStartDate()
    {
        var booking = CreateValidBooking();
        booking.EndDate = booking.StartDate;

        var (added, error) = await Sut.AddBookingAsync("emp-1", booking);

        Assert.False(added);
        Assert.Equal("EndTime früher oder gleich StartTime", error);
        RepoMock.Verify(r => r.AddBookingAsync(It.IsAny<Booking>()), Times.Never);
    }

    // Speichern schlägt fehl
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

        var (added, error) = await Sut.AddBookingAsync("emp-1", booking);

        Assert.False(added);
        Assert.Equal("Fehler beim Speichern", error);
    }

    // Overlapping Booking im gleichen Raum --> Fehler
    [Fact]
    public async Task AddBookingAsync_ReturnsError_WhenOverlappingBookingExists()
    {
        var booking = CreateValidBooking();

        var existingBooking = new Booking
        {
            Id = 99,
            EmployeeId = "someone",
            RoomId = booking.RoomId,
            StartDate = booking.StartDate.AddMinutes(-30),
            EndDate = booking.StartDate.AddMinutes(30)
        };

        RepoMock.Setup(r => r.GetBookingsByRoomIdAsync(booking.RoomId))
                .ReturnsAsync(new List<Booking> { existingBooking });

        var (added, error) = await Sut.AddBookingAsync("emp-1", booking);

        Assert.False(added);
        Assert.Equal("Buchung besteht bereits", error);
        RepoMock.Verify(r => r.AddBookingAsync(It.IsAny<Booking>()), Times.Never);
    }

    // Erfolgsfall
    [Fact]
    public async Task AddBookingAsync_ReturnsTrue_WhenValid()
    {
        var booking = CreateValidBooking();

        RepoMock.Setup(r => r.GetBookingsByRoomIdAsync(booking.RoomId))
                .ReturnsAsync(new List<Booking>());

        RepoMock.Setup(r => r.AddBookingAsync(booking))
                .ReturnsAsync(true);

        var (added, error) = await Sut.AddBookingAsync("emp-1", booking);

        Assert.True(added);
        Assert.Null(error);
        RepoMock.Verify(r => r.AddBookingAsync(booking), Times.Once);
    }
}
