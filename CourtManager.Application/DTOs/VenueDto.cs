namespace CourtManager.Application.DTOs;

public class VenueDto
{
    public Guid VenueId { get; set; }
    public Guid OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Description { get; set; } = string.Empty;
    public string OpeningHours { get; set; } = string.Empty;
    public string PhoneContact { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public IEnumerable<string> ImageUrls { get; set; } = [];
    public IEnumerable<string> Amenities { get; set; } = [];
    public IEnumerable<FootballFieldDto> Fields { get; set; } = [];
}

public class VenueQueryDto
{
    public string? Q { get; set; }
    public string? FieldType { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinRating { get; set; }
    public string? Amenity { get; set; }
    public string? SortBy { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class CreateVenueDto
{
    public string VenueName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Description { get; set; } = string.Empty;
    public string OpeningHours { get; set; } = string.Empty;
    public string PhoneContact { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public IEnumerable<string> ImageUrls { get; set; } = [];
}

public class UpdateVenueDto : CreateVenueDto
{
}
