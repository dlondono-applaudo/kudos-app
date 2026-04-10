namespace KudosApp.Core.Entities;

public class UserBadge
{
    public int Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public int BadgeId { get; private set; }
    public DateTime AwardedAt { get; private set; } = DateTime.UtcNow;

    // Navigation
    public ApplicationUser User { get; private set; } = null!;
    public Badge Badge { get; private set; } = null!;

    private UserBadge() { }

    public static UserBadge Award(string userId, int badgeId)
    {
        return new UserBadge
        {
            UserId = userId,
            BadgeId = badgeId,
            AwardedAt = DateTime.UtcNow
        };
    }
}
