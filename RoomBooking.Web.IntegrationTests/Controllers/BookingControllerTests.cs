using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RoomBooking.Domain.Models;
using Xunit;

namespace RoomBooking.Web.IntegrationTests.Controllers;

public class BookingControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BookingControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllBookings_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/Booking");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetBookingById_ReturnsNotFound_WhenInvalid()
    {
        var response = await _client.GetAsync("/api/Booking/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddBooking_ReturnsBadRequest_WhenInvalid()
    {
        var booking = new Booking
        {
            // Missing EmployeeId → your controller returns BadRequest
            RoomId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddHours(1)
        };

        var response = await _client.PostAsJsonAsync("/api/Booking", booking);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}