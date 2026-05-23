namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents an image of a venue.
/// </summary>
public class VenueImage
{
    public Guid ImageId { get; set; }
    public Guid VenueId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation property
    public Venue? Venue { get; set; }
}
