using MediatR;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Command to unlock a time slot.
/// Changes slot status from "Locked" back to "Available".
/// Used when payment fails, times out, or is refunded.
/// Implements CQRS Command pattern.
/// </summary>
public class UnlockTimeSlotCommand : IRequest<bool>
{
    /// <summary>
    /// The ID of the time slot to unlock.
    /// </summary>
    public Guid SlotId { get; set; }

    /// <summary>
    /// Reason for unlocking (e.g., "PaymentFailed", "PaymentTimeout", "Refund").
    /// </summary>
    public string UnlockReason { get; set; } = string.Empty;

    public UnlockTimeSlotCommand(Guid slotId, string unlockReason = "ManualUnlock")
    {
        SlotId = slotId;
        UnlockReason = unlockReason;
    }
}
