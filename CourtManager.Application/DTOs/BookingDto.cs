using CourtManager.Domain.Entities;

namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for Booking.
/// </summary>
public class BookingDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CourtId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus Status { get; set; }
}

/// <summary>
/// DTO for creating a new booking (request).
/// </summary>
public class CreateBookingDto
{
    public Guid UserId { get; set; }
    public Guid CourtId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
