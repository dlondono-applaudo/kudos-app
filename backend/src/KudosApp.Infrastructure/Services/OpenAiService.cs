using System.Text.Json;
using KudosApp.Core.DTOs.Ai;
using KudosApp.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace KudosApp.Infrastructure.Services;

public class OpenAiService : IAiSuggestionService, IContentModerationService
{
    private readonly ChatClient? _chatClient;
    private readonly string _model;
    private readonly ILogger<OpenAiService> _logger;

    public OpenAiService(IConfiguration configuration, ILogger<OpenAiService> logger)
    {
        _logger = logger;
        var apiKey = configuration["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        _model = configuration["OpenAI:Model"]
            ?? Environment.GetEnvironmentVariable("OPENAI_MODEL")
            ?? "gpt-4o-mini";

        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            _chatClient = new ChatClient(_model, apiKey);
            _logger.LogInformation("OpenAI service initialized with model {Model}", _model);
        }
        else
        {
            _logger.LogWarning("OpenAI API key not configured — AI features will use fallback responses");
        }
    }

    public async Task<IReadOnlyList<string>> SuggestMessagesAsync(string categoryName, string intent)
    {
        if (_chatClient is null)
        {
            return new List<string>
            {
                $"Great job on {intent}! Your {categoryName.ToLower()} skills are amazing! 🌟",
                $"Thanks for {intent}! You truly embody {categoryName.ToLower()}! 💪",
                $"Your work on {intent} was outstanding! Keep it up! 🚀"
            };
        }

        var systemPrompt = """
            You are a workplace kudos message writer. Generate exactly 3 short, 
            enthusiastic recognition messages (1-2 sentences each) with relevant emojis.
            Each message should feel genuine, professional, and warm.
            Return ONLY a JSON array of 3 strings. No markdown, no code fences.
            """;

        var userPrompt = $"Category: {categoryName}\nIntent: {intent}";

        try
        {
            var response = await _chatClient.CompleteChatAsync(
                new List<ChatMessage>
                {
                    new SystemChatMessage(systemPrompt),
                    new UserChatMessage(userPrompt)
                },
                new ChatCompletionOptions { Temperature = 0.8f });

            var content = response.Value.Content[0].Text.Trim();
            var suggestions = JsonSerializer.Deserialize<List<string>>(content);
            return suggestions?.Count > 0 ? suggestions : FallbackSuggestions(categoryName, intent);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI suggestion generation failed, using fallback");
            return FallbackSuggestions(categoryName, intent);
        }
    }

    public async Task<ModerationResult> ValidateAndAnalyzeAsync(string message, string categoryName)
    {
        if (_chatClient is null)
        {
            return new ModerationResult(true, null, "🌟");
        }

        var systemPrompt = """
            You are a content moderator AND sentiment analyzer for a workplace kudos app.
            Analyze the message and return a JSON object with exactly these fields:
            - "approved": boolean — false only if the message contains profanity, hate speech, 
              harassment, threats, or inappropriate content for a workplace
            - "reason": string or null — if not approved, explain why briefly
            - "emoji": string — a single emoji that captures the sentiment/mood of the message.
              Choose from: 🔥 (passionate/intense), 💪 (strength/determination), 🌟 (excellence), 
              ❤️ (heartfelt/caring), 🎯 (precision/focused), 🚀 (growth/momentum), 
              👏 (celebration), 🙌 (gratitude), 🧠 (smart/clever), 🤝 (collaboration)
            Return ONLY the JSON object. No markdown, no code fences.
            """;

        var userPrompt = $"Category: {categoryName}\nMessage: {message}";

        try
        {
            var response = await _chatClient.CompleteChatAsync(
                new List<ChatMessage>
                {
                    new SystemChatMessage(systemPrompt),
                    new UserChatMessage(userPrompt)
                },
                new ChatCompletionOptions { Temperature = 0.1f });

            var content = response.Value.Content[0].Text.Trim();
            var result = JsonSerializer.Deserialize<ModerationJson>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
                return new ModerationResult(true, null, "🌟");

            return new ModerationResult(result.Approved, result.Reason, result.Emoji ?? "🌟");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI moderation failed, auto-approving");
            return new ModerationResult(true, null, "🌟");
        }
    }

    private static IReadOnlyList<string> FallbackSuggestions(string categoryName, string intent)
    {
        return new List<string>
        {
            $"Great job on {intent}! Your {categoryName.ToLower()} skills are amazing! 🌟",
            $"Thanks for {intent}! You truly embody {categoryName.ToLower()}! 💪",
            $"Your work on {intent} was outstanding! Keep it up! 🚀"
        };
    }

    private record ModerationJson(bool Approved, string? Reason, string? Emoji);
}
