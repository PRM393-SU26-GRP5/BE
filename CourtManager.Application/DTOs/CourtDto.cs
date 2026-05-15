namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for Court.
/// </summary>
public class CourtDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal PricePerHour { get; set; }
    public bool IsAvailable { get; set; }
}
