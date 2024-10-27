namespace ArchiBase.Models;

public interface IAuditable
{
    Guid Id { get; set; }
}

public class AuditRecord
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }

    public string EntityType { get; set; }

    public Guid UserId { get; set; }

    public string Action { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public AuditRecord()
    {
        EntityId = Guid.Empty;
        EntityType = "";
        Action = "";
    }

    public AuditRecord(IAuditable entity, Guid userId, string action = "")
    {
        EntityId = entity.Id;
        EntityType = entity.GetType().Name ?? "";
        UserId = userId;
        Action = action;
    }
}