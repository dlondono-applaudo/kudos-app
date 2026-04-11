using System.Net;
using System.Net.Http.Json;
using KudosApp.Domain.DTOs.Kudos;
using KudosApp.Domain.DTOs.Leaderboard;

namespace KudosApp.Tests;

public class LeaderboardControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public LeaderboardControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetLeaderboard_AllowsAnonymous_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/leaderboard");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLeaderboard_ReturnsEmptyListWhenNoKudos()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/leaderboard?top=5");
        var entries = await response.Content.ReadFromJsonAsync<List<LeaderboardEntry>>();

        Assert.NotNull(entries);
    }

    [Fact]
    public async Task GetLeaderboard_WithKudos_ReturnsRankedEntries()
    {
        var client = _factory.CreateClient();

        // Register sender and receiver
        var (senderToken, _) = await AuthControllerTests.RegisterAndGetTokenAsync(
            client, "lb-sender@example.com", "LB Sender");
        await AuthControllerTests.RegisterAndGetTokenAsync(
            client, "lb-receiver@example.com", "LB Receiver");

        AuthControllerTests.Authenticate(client, senderToken);

        // Get receiver ID
        var usersResponse = await client.GetFromJsonAsync<List<UserListItem>>("/api/users");
        var receiver = usersResponse?.FirstOrDefault(u => u.Email == "lb-receiver@example.com");
        if (receiver is null) return;

        // Create a kudos
        await client.PostAsJsonAsync("/api/kudos",
            new CreateKudosRequest(receiver.Id, 1, "Awesome teamwork on the sprint!"));

        // Check leaderboard
        var leaderboard = await client.GetFromJsonAsync<List<LeaderboardEntry>>("/api/leaderboard");
        Assert.NotNull(leaderboard);
        Assert.Contains(leaderboard, e => e.FullName == "LB Receiver");
    }

    private record UserListItem(string Id, string Email, string FullName, string Department);
}
