namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for creating a booking with multiple time slots.
/// Used for bulk slot bookings from shopping cart.
/// </summary>
public class BookingRequestDto
{
    /// <summary>
    /// Foreign key referencing the football field to be booked.
    /// </summary>
    public Guid FieldId { get; set; }

    /// <summary>
    /// Array of time slot IDs to be booked together.
    /// </summary>
    public Guid[] SlotIds { get; set; } = Array.Empty<Guid>();

    /// <summary>
    /// Optional note or special request for the booking.
    /// </summary>
    public string? Note { get; set; }
}
