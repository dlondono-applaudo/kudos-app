using KudosApp.Domain.DTOs.Leaderboard;

namespace KudosApp.Domain.Interfaces;

public interface ILeaderboardService
{
    Task<IReadOnlyList<LeaderboardEntry>> GetLeaderboardAsync(int top);
}
