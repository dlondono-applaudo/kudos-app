using KudosApp.Core.Entities;

namespace KudosApp.Tests;

public class KudosEntityTests
{
    [Fact]
    public void Create_WithValidData_ReturnsKudos()
    {
        var kudos = Kudos.Create("sender-1", "receiver-1", 1, "Great work!", 10);

        Assert.Equal("sender-1", kudos.SenderId);
        Assert.Equal("receiver-1", kudos.ReceiverId);
        Assert.Equal(1, kudos.CategoryId);
        Assert.Equal("Great work!", kudos.Message);
        Assert.Equal(10, kudos.Points);
        Assert.Null(kudos.SentimentEmoji);
    }

    [Fact]
    public void Create_SelfKudos_ThrowsInvalidOperation()
    {
        var ex = Assert.Throws<InvalidOperationException>(
            () => Kudos.Create("user-1", "user-1", 1, "Self praise", 10));

        Assert.Contains("yourself", ex.Message);
    }

    [Fact]
    public void Create_EmptyMessage_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => Kudos.Create("sender-1", "receiver-1", 1, "", 10));
    }

    [Fact]
    public void Create_WhitespaceMessage_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => Kudos.Create("sender-1", "receiver-1", 1, "   ", 10));
    }

    [Fact]
    public void SetSentimentEmoji_SetsValue()
    {
        var kudos = Kudos.Create("sender-1", "receiver-1", 1, "Great!", 10);
        kudos.SetSentimentEmoji("🔥");

        Assert.Equal("🔥", kudos.SentimentEmoji);
    }
}
