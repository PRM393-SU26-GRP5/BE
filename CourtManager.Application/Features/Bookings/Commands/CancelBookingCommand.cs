using MediatR;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Command to cancel an existing booking.
/// Can cancel Pending or Confirmed bookings.
/// Implements CQRS Command pattern.
/// </summary>
public class CancelBookingCommand : IRequest<bool>
{
    /// <summary>
    /// The ID of the booking to cancel.
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// Optional reason for cancellation.
    /// </summary>
    public string? CancellationReason { get; set; }

    public CancelBookingCommand(Guid bookingId, string? cancellationReason = null)
    {
        BookingId = bookingId;
        CancellationReason = cancellationReason;
    }
}
