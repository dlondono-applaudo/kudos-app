using KudosApp.Core.DTOs.Leaderboard;
using KudosApp.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KudosApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public LeaderboardController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LeaderboardEntry>>> GetLeaderboard(
        [FromQuery] int top = 10)
    {
        if (top is < 1 or > 100) top = 10;

        var entries = await _context.Kudos
            .GroupBy(k => k.ReceiverId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalPoints = g.Sum(k => k.Points),
                KudosReceived = g.Count()
            })
            .OrderByDescending(x => x.TotalPoints)
            .Take(top)
            .ToListAsync();

        var userIds = entries.Select(e => e.UserId).ToList();
        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id);

        var leaderboard = entries.Select((e, index) => new LeaderboardEntry(
            e.UserId,
            users.TryGetValue(e.UserId, out var user) ? user.FullName : "Unknown",
            users.TryGetValue(e.UserId, out var u) ? u.Department : "Unknown",
            e.TotalPoints,
            e.KudosReceived,
            index + 1
        )).ToList();

        return Ok(leaderboard);
    }
}
