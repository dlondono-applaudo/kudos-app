using KudosApp.Domain.DTOs.Notifications;

namespace KudosApp.Domain.Interfaces;

public interface INotificationsService
{
    Task<IReadOnlyList<NotificationResponse>> GetByUserAsync(string userId);
    Task<int> GetUnreadCountAsync(string userId);
    Task<bool> MarkAsReadAsync(int id, string userId);
    Task MarkAllAsReadAsync(string userId);
}
