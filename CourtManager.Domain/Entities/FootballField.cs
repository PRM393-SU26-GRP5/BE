namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a football field available for booking.
/// </summary>
public class FootballField
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal PricePerHour { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public User? Owner { get; set; }
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<FieldImage> FieldImages { get; set; } = [];
    public ICollection<TimeSlot> TimeSlots { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
