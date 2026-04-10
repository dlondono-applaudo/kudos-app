using System.ComponentModel.DataAnnotations;

namespace KudosApp.Core.DTOs.Ai;

public record SuggestMessageRequest(
    [Required, MinLength(1)] string CategoryName,
    [Required, MinLength(3), MaxLength(200)] string Intent
);

public record SuggestMessageResponse(
    IReadOnlyList<string> Suggestions
);

public record ModerationResult(
    bool IsApproved,
    string? Reason,
    string? SentimentEmoji
);
