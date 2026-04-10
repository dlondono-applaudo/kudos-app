using System.Net;
using System.Net.Http.Json;
using KudosApp.Core.DTOs.Kudos;
using KudosApp.Core.DTOs.Notifications;

namespace KudosApp.Tests;

public class NotificationsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public NotificationsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetNotifications_Unauthenticated_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/notifications");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetNotifications_Authenticated_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var (token, _) = await AuthControllerTests.RegisterAndGetTokenAsync(
            client, "notif-user@example.com", "Notif User");
        AuthControllerTests.Authenticate(client, token);

        var response = await client.GetAsync("/api/notifications");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUnreadCount_Authenticated_ReturnsNumber()
    {
        var client = _factory.CreateClient();
        var (token, _) = await AuthControllerTests.RegisterAndGetTokenAsync(
            client, "unread-user@example.com", "Unread User");
        AuthControllerTests.Authenticate(client, token);

        var response = await client.GetAsync("/api/notifications/unread-count");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var count = await response.Content.ReadFromJsonAsync<int>();
        Assert.True(count >= 0);
    }

    [Fact]
    public async Task MarkAsRead_NonExistent_ReturnsNotFound()
    {
        var client = _factory.CreateClient();
        var (token, _) = await AuthControllerTests.RegisterAndGetTokenAsync(
            client, "mark-user@example.com", "Mark User");
        AuthControllerTests.Authenticate(client, token);

        var response = await client.PostAsync("/api/notifications/99999/read", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task MarkAllAsRead_Authenticated_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var (token, _) = await AuthControllerTests.RegisterAndGetTokenAsync(
            client, "markall-user@example.com", "MarkAll User");
        AuthControllerTests.Authenticate(client, token);

        var response = await client.PostAsync("/api/notifications/read-all", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Notification_CreatedWhenKudosSent()
    {
        var client = _factory.CreateClient();

        // Register sender and receiver
        var (senderToken, _) = await AuthControllerTests.RegisterAndGetTokenAsync(
            client, "notif-sender@example.com", "Notif Sender");
        var (receiverToken, _) = await AuthControllerTests.RegisterAndGetTokenAsync(
            client, "notif-receiver@example.com", "Notif Receiver");

        // Get receiver ID
        AuthControllerTests.Authenticate(client, senderToken);
        var users = await client.GetFromJsonAsync<List<UserListItem>>("/api/users");
        var receiver = users?.FirstOrDefault(u => u.Email == "notif-receiver@example.com");
        if (receiver is null) return;

        // Send kudos
        await client.PostAsJsonAsync("/api/kudos",
            new CreateKudosRequest(receiver.Id, 1, "Great collaboration today!"));

        // Check receiver notifications
        AuthControllerTests.Authenticate(client, receiverToken);
        var notifications = await client.GetFromJsonAsync<List<NotificationResponse>>("/api/notifications");

        Assert.NotNull(notifications);
        Assert.NotEmpty(notifications);
        Assert.Contains(notifications, n => n.Message.Contains("kudos"));
    }

    private record UserListItem(string Id, string Email, string FullName, string Department);
}
