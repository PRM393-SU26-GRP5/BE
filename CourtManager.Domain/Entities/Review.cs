namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a user review for a football field.
/// </summary>
public class Review
{
    public Guid ReviewId { get; set; }
    public Guid UserId { get; set; }
    public Guid VenueId { get; set; }
    public Guid BookingId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Venue? Venue { get; set; }
    public Booking? Booking { get; set; }
}
