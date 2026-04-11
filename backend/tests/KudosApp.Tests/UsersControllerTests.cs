using System.Net;
using System.Net.Http.Json;
using KudosApp.Domain.DTOs.Auth;
using KudosApp.Domain.DTOs.Users;

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

    [Fact]
    public async Task CreateUser_AsAdmin_ReturnsCreated()
    {
        var client = _factory.CreateClient();
        var adminToken = await LoginAsAdminAsync(client);
        AuthControllerTests.Authenticate(client, adminToken);

        var request = new RegisterRequest("newuser@example.com", "Test123!", "New User", "Marketing");
        var response = await client.PostAsJsonAsync("/api/users", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_AsRegularUser_ReturnsForbidden()
    {
        var client = _factory.CreateClient();
        var (token, _) = await AuthControllerTests.RegisterAndGetTokenAsync(
            client, "regularuser-create@example.com", "Regular User");
        AuthControllerTests.Authenticate(client, token);

        var request = new RegisterRequest("another@example.com", "Test123!", "Another User", "Sales");
        var response = await client.PostAsJsonAsync("/api/users", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_ReturnsConflict()
    {
        var client = _factory.CreateClient();
        var adminToken = await LoginAsAdminAsync(client);
        AuthControllerTests.Authenticate(client, adminToken);

        var request = new RegisterRequest("dupuser@example.com", "Test123!", "Dup User", "HR");
        await client.PostAsJsonAsync("/api/users", request);
        var response = await client.PostAsJsonAsync("/api/users", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_InvalidRequest_ReturnsValidationError()
    {
        var client = _factory.CreateClient();
        var adminToken = await LoginAsAdminAsync(client);
        AuthControllerTests.Authenticate(client, adminToken);

        var request = new RegisterRequest("", "", "", "");
        var response = await client.PostAsJsonAsync("/api/users", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_Unauthenticated_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var request = new RegisterRequest("unauth@example.com", "Test123!", "Unauth User", "IT");
        var response = await client.PostAsJsonAsync("/api/users", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static async Task<string> LoginAsAdminAsync(HttpClient client)
    {
        var login = new LoginRequest("admin@kudos.com", "Admin123!");
        var response = await client.PostAsJsonAsync("/api/auth/login", login);
        response.EnsureSuccessStatusCode();
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return auth!.Token;
    }
}
