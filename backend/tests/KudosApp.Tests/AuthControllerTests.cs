using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using KudosApp.Core.DTOs.Auth;

namespace KudosApp.Tests;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOkWithToken()
    {
        var request = new RegisterRequest("test@example.com", "Test123!", "Test User", "Engineering");

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));
        Assert.Equal("test@example.com", auth.Email);
        Assert.Contains("User", auth.Roles);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        var request = new RegisterRequest("dup@example.com", "Test123!", "Dup User", "Engineering");

        await _client.PostAsJsonAsync("/api/auth/register", request);
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var reg = new RegisterRequest("login@example.com", "Login123!", "Login User", "Engineering");
        await _client.PostAsJsonAsync("/api/auth/register", reg);

        var login = new LoginRequest("login@example.com", "Login123!");
        var response = await _client.PostAsJsonAsync("/api/auth/login", login);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var reg = new RegisterRequest("wrong@example.com", "Correct123!", "WP User", "Engineering");
        await _client.PostAsJsonAsync("/api/auth/register", reg);

        var login = new LoginRequest("wrong@example.com", "BadPass123!");
        var response = await _client.PostAsJsonAsync("/api/auth/login", login);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    internal static async Task<(string Token, string UserId)> RegisterAndGetTokenAsync(
        HttpClient client, string email, string fullName = "Test User")
    {
        var reg = new RegisterRequest(email, "Test123!", fullName, "Engineering");
        var response = await client.PostAsJsonAsync("/api/auth/register", reg);
        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return (auth!.Token, auth.Email);
    }

    internal static void Authenticate(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
}
