using KudosApp.Core.DTOs.Kudos;
using KudosApp.Core.Entities;
using KudosApp.Core.Interfaces;
using KudosApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KudosApp.Infrastructure.Services;

public class KudosService : IKudosService
{
    private readonly AppDbContext _context;

    public KudosService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<KudosResponse> CreateAsync(string senderId, CreateKudosRequest request)
    {
        var category = await _context.Categories.FindAsync(request.CategoryId)
            ?? throw new ArgumentException("Category not found.");

        var kudos = Kudos.Create(
            senderId,
            request.ReceiverId,
            request.CategoryId,
            request.Message,
            category.PointValue);

        _context.Kudos.Add(kudos);

        // Create notification for receiver
        var sender = await _context.Users.FindAsync(senderId);
        var notification = Notification.Create(
            request.ReceiverId,
            $"{sender?.FullName ?? "Someone"} sent you kudos for {category.Name}!");
        _context.Notifications.Add(notification);

        // Create audit log
        _context.AuditLogs.Add(AuditLog.Create(senderId, "Create", "Kudos", kudos.Id.ToString()));

        await _context.SaveChangesAsync();

        // Check and award badges
        await CheckAndAwardBadgesAsync(request.ReceiverId);

        return await MapToResponse(kudos);
    }

    public async Task<KudosFeedResponse> GetFeedAsync(int page, int pageSize)
    {
        var totalCount = await _context.Kudos.CountAsync();

        var items = await _context.Kudos
            .Include(k => k.Sender)
            .Include(k => k.Receiver)
            .Include(k => k.Category)
            .OrderByDescending(k => k.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(k => new KudosResponse(
                k.Id,
                k.SenderId,
                k.Sender.FullName,
                k.ReceiverId,
                k.Receiver.FullName,
                k.Category.Name,
                k.Message,
                k.Points,
                k.CreatedAt))
            .ToListAsync();

        return new KudosFeedResponse(items, totalCount, page, pageSize);
    }

    public async Task<KudosResponse?> GetByIdAsync(int id)
    {
        var kudos = await _context.Kudos
            .Include(k => k.Sender)
            .Include(k => k.Receiver)
            .Include(k => k.Category)
            .FirstOrDefaultAsync(k => k.Id == id);

        return kudos is null ? null : MapToResponseDirect(kudos);
    }

    public async Task<IReadOnlyList<KudosResponse>> GetBySenderAsync(string userId)
    {
        return await _context.Kudos
            .Include(k => k.Sender).Include(k => k.Receiver).Include(k => k.Category)
            .Where(k => k.SenderId == userId)
            .OrderByDescending(k => k.CreatedAt)
            .Select(k => new KudosResponse(
                k.Id, k.SenderId, k.Sender.FullName,
                k.ReceiverId, k.Receiver.FullName,
                k.Category.Name, k.Message, k.Points, k.CreatedAt))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<KudosResponse>> GetByReceiverAsync(string userId)
    {
        return await _context.Kudos
            .Include(k => k.Sender).Include(k => k.Receiver).Include(k => k.Category)
            .Where(k => k.ReceiverId == userId)
            .OrderByDescending(k => k.CreatedAt)
            .Select(k => new KudosResponse(
                k.Id, k.SenderId, k.Sender.FullName,
                k.ReceiverId, k.Receiver.FullName,
                k.Category.Name, k.Message, k.Points, k.CreatedAt))
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var kudos = await _context.Kudos.FindAsync(id);
        if (kudos is null) return false;

        _context.Kudos.Remove(kudos);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task CheckAndAwardBadgesAsync(string userId)
    {
        var totalPoints = await _context.Kudos
            .Where(k => k.ReceiverId == userId)
            .SumAsync(k => k.Points);

        var existingBadgeIds = await _context.UserBadges
            .Where(ub => ub.UserId == userId)
            .Select(ub => ub.BadgeId)
            .ToListAsync();

        var newBadges = await _context.Badges
            .Where(b => b.RequiredPoints <= totalPoints && !existingBadgeIds.Contains(b.Id))
            .ToListAsync();

        foreach (var badge in newBadges)
        {
            _context.UserBadges.Add(UserBadge.Award(userId, badge.Id));
            _context.Notifications.Add(Notification.Create(userId, $"You earned the \"{badge.Name}\" badge!"));
        }

        if (newBadges.Count > 0)
            await _context.SaveChangesAsync();
    }

    private async Task<KudosResponse> MapToResponse(Kudos kudos)
    {
        await _context.Entry(kudos).Reference(k => k.Sender).LoadAsync();
        await _context.Entry(kudos).Reference(k => k.Receiver).LoadAsync();
        await _context.Entry(kudos).Reference(k => k.Category).LoadAsync();
        return MapToResponseDirect(kudos);
    }

    private static KudosResponse MapToResponseDirect(Kudos kudos)
    {
        return new KudosResponse(
            kudos.Id,
            kudos.SenderId,
            kudos.Sender.FullName,
            kudos.ReceiverId,
            kudos.Receiver.FullName,
            kudos.Category.Name,
            kudos.Message,
            kudos.Points,
            kudos.CreatedAt);
    }
}
