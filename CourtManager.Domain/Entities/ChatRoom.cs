namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a chat room between a customer and a field host.
/// </summary>
public class ChatRoom
{
    public Guid RoomId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid HostId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public User? Customer { get; set; }
    public User? Host { get; set; }
    public ICollection<Message> Messages { get; set; } = [];
}
