namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a booking of a football field by a user.
/// </summary>
public class Booking
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FieldId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public string BookingStatus { get; set; } = "Pending";
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public FootballField? Field { get; set; }
    public ICollection<BookingItem> BookingItems { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
}
