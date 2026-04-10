namespace KudosApp.Core.DTOs.Notifications;

public record NotificationResponse(
    int Id,
    string Message,
    bool IsRead,
    DateTime CreatedAt
);
