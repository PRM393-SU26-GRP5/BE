namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a single time slot item within a booking.
/// </summary>
public class BookingItem
{
    public Guid BookingItemId { get; set; }
    public Guid BookingId { get; set; }
    public Guid SlotId { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Booking? Booking { get; set; }
    public TimeSlot? Slot { get; set; }
}
