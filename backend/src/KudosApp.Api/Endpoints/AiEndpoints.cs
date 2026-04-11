using FluentValidation;
using KudosApp.Domain.DTOs.Ai;
using KudosApp.Domain.Interfaces;

namespace KudosApp.Api.Endpoints;

public static class AiEndpoints
{
    public static IEndpointRouteBuilder MapAiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/ai").WithTags("AI").RequireAuthorization();

        group.MapPost("suggest-message", async (
            SuggestMessageRequest request,
            IValidator<SuggestMessageRequest> validator,
            IAiSuggestionService suggestionService) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());

            var suggestions = await suggestionService.SuggestMessagesAsync(request.CategoryName, request.Intent);
            return Results.Ok(new SuggestMessageResponse(suggestions));
        });

        return app;
    }
}
