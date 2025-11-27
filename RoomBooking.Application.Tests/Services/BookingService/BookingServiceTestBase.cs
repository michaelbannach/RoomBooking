using Microsoft.Extensions.Logging;
using Moq;
using RoomBooking.Application.Interfaces;
using RoomBooking.Domain.Models;

namespace RoomBooking.Application.Tests.Services.BookingService;

public abstract class BookingServiceTestBase
{
    protected readonly Mock<IBookingRepository> RepoMock;
    protected readonly Mock<ILogger<RoomBooking.Application.Services.BookingService>> LoggerMock;
    protected readonly RoomBooking.Application.Services.BookingService Sut;

    protected BookingServiceTestBase()
    {
        RepoMock = new Mock<IBookingRepository>();
        LoggerMock = new Mock<ILogger<RoomBooking.Application.Services.BookingService>>();
        Sut = new RoomBooking.Application.Services.BookingService(RepoMock.Object, LoggerMock.Object);
    }

    protected Booking CreateValidBooking()
    {
        return new Booking
        {
            Id = 1,
            UserId = 1,
            RoomId = 1,
            StartDate = DateTime.Now.AddHours(1),
            EndDate = DateTime.Now.AddHours(2)
        };
    }
}