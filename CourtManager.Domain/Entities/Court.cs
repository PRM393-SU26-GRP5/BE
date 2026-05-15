namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a sports court available for booking.
/// </summary>
public class Court
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal PricePerHour { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsAvailable { get; set; }

    // Navigation property
    public ICollection<Booking> Bookings { get; set; } = [];
}
