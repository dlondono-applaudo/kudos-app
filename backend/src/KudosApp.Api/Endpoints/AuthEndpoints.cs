using System.Security.Claims;
using FluentValidation;
using KudosApp.Domain.DTOs.Auth;
using KudosApp.Domain.Entities;
using KudosApp.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace KudosApp.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/auth").WithTags("Auth");

        group.MapPost("register", async (
            RegisterRequest request,
            IValidator<RegisterRequest> validator,
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService) =>
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

            var roles = await userManager.GetRolesAsync(user);
            var token = tokenService.GenerateToken(user, roles);

            return Results.Ok(new AuthResponse(token, DateTime.UtcNow.AddHours(1), user.Email!, user.FullName, roles));
        });

        group.MapPost("login", async (
            LoginRequest request,
            IValidator<LoginRequest> validator,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Results.Json(new { message = "Invalid credentials." }, statusCode: 401);

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
                return Results.Json(new { message = "Invalid credentials." }, statusCode: 401);

            var roles = await userManager.GetRolesAsync(user);
            var token = tokenService.GenerateToken(user, roles);

            return Results.Ok(new AuthResponse(token, DateTime.UtcNow.AddHours(1), user.Email!, user.FullName, roles));
        });

        return app;
    }
}
