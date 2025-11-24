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

    [Fact]
    public async Task AddRoom_ReturnsBadRequest_WhenInvalid()
    {
        var room = new Room { Id = 1, Name = "", Capacity = 0 };

        var response = await _client.PostAsJsonAsync("/api/Room", room);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task AddRoom_WithValidData_ReturnsSuccess_AndPersistsRoom()
    {
        // Arrange
        var uniqueName = $"IntegrationTestRoom-{Guid.NewGuid()}";
        var room = new Room
        {
           
            Name = uniqueName,
            Capacity = 10
        };

       
        var postResponse = await _client.PostAsJsonAsync("/api/Room", room);

        // Assert Statuscode
        postResponse.StatusCode
            .Should()
            .BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);

        
        var allRoomsResponse = await _client.GetAsync("/api/Room");
        allRoomsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var rooms = await allRoomsResponse.Content.ReadFromJsonAsync<List<Room>>();

        rooms.Should().NotBeNull();
        rooms!
            .Any(r => r.Name == uniqueName && r.Capacity == 10)
            .Should().BeTrue("der neu angelegte Raum sollte per GET wieder auffindbar sein");
    }

}