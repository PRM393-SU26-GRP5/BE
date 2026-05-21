using MediatR;

namespace CourtManager.Application.Features.TimeSlots.Commands;

/// <summary>
/// Command to delete a time slot (soft delete).
/// </summary>
public class DeleteTimeSlotCommand : IRequest<bool>
{
    /// <summary>
    /// The ID of the time slot to delete.
    /// </summary>
    public Guid SlotId { get; set; }

    public DeleteTimeSlotCommand(Guid slotId)
    {
        SlotId = slotId;
    }
}
