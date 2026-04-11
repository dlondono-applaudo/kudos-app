using System.Security.Claims;
using KudosApp.Domain.Interfaces;

namespace KudosApp.Api.Endpoints;

public static class NotificationsEndpoints
{
    public static IEndpointRouteBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/notifications").WithTags("Notifications").RequireAuthorization();

        group.MapGet("", async (INotificationsService svc, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Results.Ok(await svc.GetByUserAsync(userId));
        });

        group.MapGet("unread-count", async (INotificationsService svc, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Results.Ok(await svc.GetUnreadCountAsync(userId));
        });

        group.MapPost("{id:int}/read", async (int id, INotificationsService svc, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return await svc.MarkAsReadAsync(id, userId) ? Results.NoContent() : Results.NotFound();
        });

        group.MapPost("read-all", async (INotificationsService svc, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await svc.MarkAllAsReadAsync(userId);
            return Results.NoContent();
        });

        return app;
    }
}
