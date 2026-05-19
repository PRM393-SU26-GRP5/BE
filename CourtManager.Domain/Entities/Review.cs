namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a user review for a football field.
/// </summary>
public class Review
{
    public Guid ReviewId { get; set; }
    public Guid UserId { get; set; }
    public Guid FieldId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public FootballField? Field { get; set; }
}
