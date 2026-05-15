namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a booking of a court by a user for a specific time period.
/// </summary>
public class Booking
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CourtId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Court? Court { get; set; }
    public Payment? Payment { get; set; }
}

/// <summary>
/// Enumeration for booking status.
/// </summary>
public enum BookingStatus
{
    Pending = 0,
    Confirmed = 1,
    Completed = 2,
    Cancelled = 3
}
