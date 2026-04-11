namespace KudosApp.Domain.Entities;

public class AuditLog
{
    public int Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public string EntityId { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private AuditLog() { }

    public static AuditLog Create(string userId, string action, string entityType, string entityId)
    {
        return new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
