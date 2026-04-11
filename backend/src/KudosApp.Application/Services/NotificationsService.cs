using KudosApp.Domain.DTOs.Notifications;
using KudosApp.Domain.Interfaces;
using KudosApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KudosApp.Application.Services;

public class NotificationsService : INotificationsService
{
    private readonly AppDbContext _context;

    public NotificationsService(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<NotificationResponse>> GetByUserAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .Select(n => new NotificationResponse(n.Id, n.Message, n.IsRead, n.CreatedAt))
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task<bool> MarkAsReadAsync(int id, string userId)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        if (notification is null) return false;
        notification.MarkAsRead();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var unread = await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();
        foreach (var n in unread) n.MarkAsRead();
        await _context.SaveChangesAsync();
    }
}
