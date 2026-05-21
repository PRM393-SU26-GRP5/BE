namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents an image associated with a football field.
/// </summary>
public class FieldImage
{
    public Guid ImageId { get; set; }
    public Guid FieldId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation property
    public FootballField? Field { get; set; }
}
