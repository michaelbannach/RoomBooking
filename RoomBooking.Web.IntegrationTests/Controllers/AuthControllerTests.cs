using System;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RoomBooking.Web.Dtos.Auth;
using Xunit;

namespace RoomBooking.Web.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ReturnsOk_WithValidCredentials()
    {
        
        var email = $"{Guid.NewGuid():N}@test.local";
        var password = "Test123!";

        
        var registerDto = new RegisterDto
        {
            Email = email,
            Password = password,
            FirstName = "Test",
            LastName = "User"
        };

        var registerResponse =
            await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginDto = new LoginDto
        {
            Email = email,
            Password = password
        };

        var loginResponse =
            await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

      
    }
}