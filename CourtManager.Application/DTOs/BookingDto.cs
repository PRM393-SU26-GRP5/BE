namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for Booking.
/// </summary>
public class BookingDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FieldId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public string? Note { get; set; }
}

/// <summary>
/// DTO for creating a new booking (request).
/// </summary>
public class CreateBookingDto
{
    public Guid UserId { get; set; }
    public Guid FieldId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
