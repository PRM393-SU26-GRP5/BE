namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a notification sent to a user.
/// </summary>
public class Notification
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public User? User { get; set; }
}
