namespace KudosApp.Domain.DTOs.Kudos;

public record CreateKudosRequest(string ReceiverId, int CategoryId, string Message);

public record KudosResponse(
    int Id, string SenderId, string SenderName, string ReceiverId, string ReceiverName,
    string CategoryName, string Message, int Points, string? SentimentEmoji, DateTime CreatedAt);

public record KudosFeedResponse(IReadOnlyList<KudosResponse> Items, int TotalCount, int Page, int PageSize);
