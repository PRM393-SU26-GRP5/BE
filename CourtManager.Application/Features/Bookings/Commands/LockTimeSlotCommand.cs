using MediatR;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Command to lock a time slot during payment process.
/// Changes slot status from "Available" to "Locked".
/// Implements CQRS Command pattern.
/// </summary>
public class LockTimeSlotCommand : IRequest<bool>
{
    /// <summary>
    /// The ID of the time slot to lock.
    /// </summary>
    public Guid SlotId { get; set; }

    /// <summary>
    /// The booking ID this lock is associated with (for reference).
    /// </summary>
    public Guid BookingId { get; set; }

    public LockTimeSlotCommand(Guid slotId, Guid bookingId)
    {
        SlotId = slotId;
        BookingId = bookingId;
    }
}
