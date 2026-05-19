namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a bookable time slot for a football field.
/// </summary>
public class TimeSlot
{
    public Guid SlotId { get; set; }
    public Guid FieldId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string SlotStatus { get; set; } = "Available";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public FootballField? Field { get; set; }
    public ICollection<BookingItem> BookingItems { get; set; } = [];
}
