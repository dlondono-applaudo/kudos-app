using System.Net;
using System.Net.Http.Json;
using KudosApp.Core.DTOs.Users;

namespace KudosApp.Tests;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAllUsers_Unauthenticated_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllUsers_Authenticated_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var (token, _) = await AuthControllerTests.RegisterAndGetTokenAsync(
            client, "allusers@example.com", "All Users");
        AuthControllerTests.Authenticate(client, token);

        var response = await client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var users = await response.Content.ReadFromJsonAsync<List<UserListItem>>();
        Assert.NotNull(users);
        Assert.NotEmpty(users);
    }

    [Fact]
    public async Task GetMyProfile_Authenticated_ReturnsProfile()
    {
        var client = _factory.CreateClient();
        var (token, _) = await AuthControllerTests.RegisterAndGetTokenAsync(
            client, "profile@example.com", "Profile User");
        AuthControllerTests.Authenticate(client, token);

        var response = await client.GetAsync("/api/users/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var profile = await response.Content.ReadFromJsonAsync<UserProfileResponse>();
        Assert.NotNull(profile);
        Assert.Equal("Profile User", profile.FullName);
        Assert.Equal("Engineering", profile.Department);
    }

    [Fact]
    public async Task GetMyProfile_Unauthenticated_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/users/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_ByIdAnonymous_ReturnsOk()
    {
        var client = _factory.CreateClient();

        // Register a user first to get a valid ID
        var (token, _) = await AuthControllerTests.RegisterAndGetTokenAsync(
            client, "publicprofile@example.com", "Public Profile");
        AuthControllerTests.Authenticate(client, token);

        // Get user ID from users list
        var users = await client.GetFromJsonAsync<List<UserListItem>>("/api/users");
        var user = users?.FirstOrDefault(u => u.Email == "publicprofile@example.com");
        if (user is null) return;

        // Access profile as anonymous
        var anonClient = _factory.CreateClient();
        var response = await anonClient.GetAsync($"/api/users/{user.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_NonExistentId_ReturnsNotFound()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/users/nonexistent-id-12345");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
