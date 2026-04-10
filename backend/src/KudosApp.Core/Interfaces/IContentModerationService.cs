using KudosApp.Core.DTOs.Ai;

namespace KudosApp.Core.Interfaces;

public interface IContentModerationService
{
    Task<ModerationResult> ValidateAndAnalyzeAsync(string message, string categoryName);
}
