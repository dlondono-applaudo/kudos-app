using KudosApp.Domain.DTOs.Ai;

namespace KudosApp.Domain.Interfaces;

public interface IContentModerationService
{
    Task<ModerationResult> ValidateAndAnalyzeAsync(string message, string categoryName);
}
