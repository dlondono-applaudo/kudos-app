using System.Security.Claims;
using KudosApp.Domain.Interfaces;

namespace KudosApp.Api.Endpoints;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/users").WithTags("Users").RequireAuthorization();

        group.MapGet("me", async (IUsersService usersService, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var profile = await usersService.GetProfileAsync(userId);
            return profile is null ? Results.NotFound() : Results.Ok(profile);
        });

        group.MapGet("{id}", async (string id, IUsersService usersService) =>
        {
            var profile = await usersService.GetProfileAsync(id);
            return profile is null ? Results.NotFound() : Results.Ok(profile);
        }).AllowAnonymous();

        group.MapGet("", async (IUsersService usersService) =>
            Results.Ok(await usersService.GetAllAsync()));

        return app;
    }
}
