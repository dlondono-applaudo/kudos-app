using KudosApp.Domain.DTOs.Leaderboard;
using KudosApp.Domain.Interfaces;
using KudosApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KudosApp.Application.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly AppDbContext _context;

    public LeaderboardService(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<LeaderboardEntry>> GetLeaderboardAsync(int top)
    {
        var entries = await _context.Kudos
            .GroupBy(k => k.ReceiverId)
            .Select(g => new { UserId = g.Key, TotalPoints = g.Sum(k => k.Points), KudosReceived = g.Count() })
            .OrderByDescending(x => x.TotalPoints)
            .Take(top)
            .ToListAsync();

        var userIds = entries.Select(e => e.UserId).ToList();
        var users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id);

        return entries.Select((e, index) => new LeaderboardEntry(
            e.UserId,
            users.TryGetValue(e.UserId, out var user) ? user.FullName : "Unknown",
            users.TryGetValue(e.UserId, out var u) ? u.Department : "Unknown",
            e.TotalPoints, e.KudosReceived, index + 1
        )).ToList();
    }
}
