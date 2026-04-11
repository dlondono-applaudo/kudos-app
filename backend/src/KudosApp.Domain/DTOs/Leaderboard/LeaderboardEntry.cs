namespace KudosApp.Domain.DTOs.Leaderboard;

public record LeaderboardEntry(string UserId, string FullName, string Department, int TotalPoints, int KudosReceived, int Rank);
