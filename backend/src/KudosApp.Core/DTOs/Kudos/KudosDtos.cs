using System.ComponentModel.DataAnnotations;

namespace KudosApp.Core.DTOs.Kudos;

public record CreateKudosRequest(
    [Required] string ReceiverId,
    [Required, Range(1, int.MaxValue)] int CategoryId,
    [Required, MinLength(5), MaxLength(500)] string Message
);

public record KudosResponse(
    int Id,
    string SenderId,
    string SenderName,
    string ReceiverId,
    string ReceiverName,
    string CategoryName,
    string Message,
    int Points,
    DateTime CreatedAt
);

public record KudosFeedResponse(
    IReadOnlyList<KudosResponse> Items,
    int TotalCount,
    int Page,
    int PageSize
);
