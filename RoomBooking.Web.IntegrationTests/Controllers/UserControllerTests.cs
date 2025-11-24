using System.Net;
using FluentAssertions;
using Xunit;

namespace RoomBooking.Web.IntegrationTests.Controllers;

public class UserControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UserControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/User");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetEmployeeById_ReturnsNotFound_WhenInvalid()
    {
        var response = await _client.GetAsync("/api/User/does-not-exist");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}