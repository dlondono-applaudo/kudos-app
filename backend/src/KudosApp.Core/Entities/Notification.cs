namespace KudosApp.Core.Entities;

public class Notification
{
    public int Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // Navigation
    public ApplicationUser User { get; private set; } = null!;

    private Notification() { }

    public static Notification Create(string userId, string message)
    {
        return new Notification
        {
            UserId = userId,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsRead() => IsRead = true;
}
