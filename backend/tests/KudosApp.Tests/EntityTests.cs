using KudosApp.Domain.Entities;

namespace KudosApp.Tests;

public class EntityTests
{
    [Fact]
    public void ApplicationUser_Create_SetsProperties()
    {
        var user = ApplicationUser.Create("test@example.com", "Test User", "Engineering");

        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("Test User", user.FullName);
        Assert.Equal("Engineering", user.Department);
        Assert.Null(user.AvatarUrl);
    }

    [Fact]
    public void ApplicationUser_UpdateProfile_UpdatesFields()
    {
        var user = ApplicationUser.Create("test@example.com", "Old Name", "Old Dept");

        user.UpdateProfile("New Name", "New Dept", "https://example.com/avatar.png");

        Assert.Equal("New Name", user.FullName);
        Assert.Equal("New Dept", user.Department);
        Assert.Equal("https://example.com/avatar.png", user.AvatarUrl);
    }

    [Fact]
    public void Category_Create_SetsProperties()
    {
        var category = Category.Create("Teamwork", "Collaboration spirit", 10);

        Assert.Equal("Teamwork", category.Name);
        Assert.Equal("Collaboration spirit", category.Description);
        Assert.Equal(10, category.PointValue);
    }

    [Fact]
    public void Badge_Create_SetsProperties()
    {
        var badge = Badge.Create("Star", "First kudos", "star", 10);

        Assert.Equal("Star", badge.Name);
        Assert.Equal("First kudos", badge.Description);
        Assert.Equal("star", badge.Icon);
        Assert.Equal(10, badge.RequiredPoints);
    }

    [Fact]
    public void UserBadge_Award_SetsProperties()
    {
        var userBadge = UserBadge.Award("user-1", 1);

        Assert.Equal("user-1", userBadge.UserId);
        Assert.Equal(1, userBadge.BadgeId);
        Assert.True(userBadge.AwardedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Notification_Create_SetsProperties()
    {
        var notification = Notification.Create("user-1", "You received kudos!");

        Assert.Equal("user-1", notification.UserId);
        Assert.Equal("You received kudos!", notification.Message);
        Assert.False(notification.IsRead);
    }

    [Fact]
    public void Notification_MarkAsRead_SetsIsReadTrue()
    {
        var notification = Notification.Create("user-1", "Test");

        notification.MarkAsRead();

        Assert.True(notification.IsRead);
    }

    [Fact]
    public void AuditLog_Create_SetsProperties()
    {
        var log = AuditLog.Create("user-1", "Create", "Kudos", "42");

        Assert.Equal("user-1", log.UserId);
        Assert.Equal("Create", log.Action);
        Assert.Equal("Kudos", log.EntityType);
        Assert.Equal("42", log.EntityId);
    }
}
