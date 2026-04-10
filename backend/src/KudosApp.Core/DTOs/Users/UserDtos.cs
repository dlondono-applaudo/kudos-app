namespace KudosApp.Core.DTOs.Users;

public record UserProfileResponse(
    string Id,
    string Email,
    string FullName,
    string Department,
    string? AvatarUrl,
    int TotalPointsReceived,
    int KudosSent,
    int KudosReceived,
    IReadOnlyList<BadgeResponse> Badges,
    DateTime CreatedAt
);

public record BadgeResponse(
    int Id,
    string Name,
    string Description,
    string Icon,
    DateTime AwardedAt
);

public record UserListItem(
    string Id,
    string Email,
    string FullName,
    string Department
);
