using System;
using System.Threading.Tasks;
using Xunit;

namespace RoomBooking.Application.Tests.Services.BookingService;

public class BookingService_GetBookingTests : BookingServiceTestBase
{
    [Fact]
    public async Task GetBookingByIdAsync_Throws_WhenIdLessOrEqualZero()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => Sut.GetBookingByIdAsync(0));
    }
}