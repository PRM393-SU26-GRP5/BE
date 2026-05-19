namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for FootballField.
/// </summary>
public class FootballFieldDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal PricePerHour { get; set; }
    public bool IsActive { get; set; }
}
