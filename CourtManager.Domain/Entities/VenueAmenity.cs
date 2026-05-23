namespace CourtManager.Domain.Entities;

/// <summary>
/// Join table for Venue and Amenity.
/// </summary>
public class VenueAmenity
{
    public Guid VenueId { get; set; }
    public Guid AmenityId { get; set; }

    // Navigation properties
    public Venue? Venue { get; set; }
    public Amenity? Amenity { get; set; }
}
