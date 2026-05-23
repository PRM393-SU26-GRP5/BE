namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents an amenity (e.g., wifi, parking).
/// </summary>
public class Amenity
{
    public Guid AmenityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public ICollection<VenueAmenity> VenueAmenities { get; set; } = [];
}
