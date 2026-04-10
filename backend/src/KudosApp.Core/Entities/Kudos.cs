namespace KudosApp.Core.Entities;

public class Kudos
{
    public int Id { get; private set; }
    public string SenderId { get; private set; } = string.Empty;
    public string ReceiverId { get; private set; } = string.Empty;
    public int CategoryId { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public int Points { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // Navigation properties
    public ApplicationUser Sender { get; private set; } = null!;
    public ApplicationUser Receiver { get; private set; } = null!;
    public Category Category { get; private set; } = null!;

    private Kudos() { }

    public static Kudos Create(string senderId, string receiverId, int categoryId, string message, int points)
    {
        if (senderId == receiverId)
            throw new InvalidOperationException("Cannot send kudos to yourself.");

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty.", nameof(message));

        return new Kudos
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            CategoryId = categoryId,
            Message = message,
            Points = points,
            CreatedAt = DateTime.UtcNow
        };
    }
}
