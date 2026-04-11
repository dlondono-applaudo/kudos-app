using System.Security.Claims;
using FluentValidation;
using KudosApp.Domain.DTOs.Kudos;
using KudosApp.Domain.Interfaces;

namespace KudosApp.Api.Endpoints;

public static class KudosEndpoints
{
    public static IEndpointRouteBuilder MapKudosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/kudos").WithTags("Kudos");

        group.MapPost("", async (
            CreateKudosRequest request,
            IValidator<CreateKudosRequest> validator,
            IKudosService kudosService,
            ClaimsPrincipal user) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());

            var senderId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;

            try
            {
                var result = await kudosService.CreateAsync(senderId, request);
                return Results.Created($"/api/kudos/{result.Id}", result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }).RequireAuthorization();

        group.MapGet("", async (int? page, int? pageSize, IKudosService kudosService) =>
        {
            var p = page ?? 1;
            var ps = pageSize ?? 20;
            if (p < 1) p = 1;
            if (ps is < 1 or > 50) ps = 20;
            return Results.Ok(await kudosService.GetFeedAsync(p, ps));
        }).AllowAnonymous();

        group.MapGet("{id:int}", async (int id, IKudosService kudosService) =>
        {
            var result = await kudosService.GetByIdAsync(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        }).AllowAnonymous();

        group.MapGet("sent", async (IKudosService kudosService, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Results.Ok(await kudosService.GetBySenderAsync(userId));
        }).RequireAuthorization();

        group.MapGet("received", async (IKudosService kudosService, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Results.Ok(await kudosService.GetByReceiverAsync(userId));
        }).RequireAuthorization();

        group.MapDelete("{id:int}", async (int id, IKudosService kudosService) =>
        {
            var success = await kudosService.DeleteAsync(id);
            return success ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(policy => policy.RequireRole("Admin"));

        return app;
    }
}
