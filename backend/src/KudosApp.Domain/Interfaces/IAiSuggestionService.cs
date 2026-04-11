namespace KudosApp.Domain.Interfaces;

public interface IAiSuggestionService
{
    Task<IReadOnlyList<string>> SuggestMessagesAsync(string categoryName, string intent);
}
