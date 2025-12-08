using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RoomBooking.Domain.Models;
using Xunit;

namespace RoomBooking.Web.IntegrationTests.Controllers;

public class RoomControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RoomControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllRooms_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/Room");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var rooms = await response.Content.ReadFromJsonAsync<List<Room>>();
        rooms.Should().NotBeNull();
    }

    [Fact]
    public async Task GetRoomById_ReturnsNotFound_WhenInvalid()
    {
        var response = await _client.GetAsync("/api/Room/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
}