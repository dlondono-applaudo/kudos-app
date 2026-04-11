namespace KudosApp.Domain.DTOs.Ai;

public record SuggestMessageRequest(string CategoryName, string Intent);

public record SuggestMessageResponse(IReadOnlyList<string> Suggestions);

public record ModerationResult(bool IsApproved, string? Reason, string? SentimentEmoji);
