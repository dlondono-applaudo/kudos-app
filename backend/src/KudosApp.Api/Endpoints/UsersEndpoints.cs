using System.Security.Claims;
using FluentValidation;
using KudosApp.Domain.DTOs.Auth;
using KudosApp.Domain.Entities;
using KudosApp.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

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

        group.MapPost("", async (
            RegisterRequest request,
            IValidator<RegisterRequest> validator,
            UserManager<ApplicationUser> userManager) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());

            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
                return Results.Conflict(new { message = "Email already registered." });

            var user = ApplicationUser.Create(request.Email, request.FullName, request.Department);
            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return Results.BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            await userManager.AddToRoleAsync(user, "User");

            return Results.Created($"/api/users/{user.Id}", new { user.Id, user.Email, user.FullName, request.Department });
        }).RequireAuthorization(policy => policy.RequireRole("Admin"));

        return app;
    }
}
