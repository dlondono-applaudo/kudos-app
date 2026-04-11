using KudosApp.Domain.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace KudosApp.Api.Endpoints;

public static class LeaderboardEndpoints
{
    public static IEndpointRouteBuilder MapLeaderboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/leaderboard").WithTags("Leaderboard");

        group.MapGet("", [OutputCache(Duration = 900)] async (int? top, ILeaderboardService leaderboardService) =>
        {
            var count = top ?? 10;
            if (count is < 1 or > 100) count = 10;
            return Results.Ok(await leaderboardService.GetLeaderboardAsync(count));
        });

        return app;
    }
}
