namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for FieldImage entity.
/// Represents an image URL associated with a football field.
/// </summary>
public class FieldImageDto
{
    /// <summary>
    /// Unique identifier for the field image.
    /// </summary>
    public Guid ImageId { get; set; }

    /// <summary>
    /// Foreign key referencing the football field.
    /// </summary>
    public Guid FieldId { get; set; }

    /// <summary>
    /// URL path or reference to the image.
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;
}
