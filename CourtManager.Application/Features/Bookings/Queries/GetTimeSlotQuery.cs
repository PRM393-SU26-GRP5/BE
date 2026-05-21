using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Query to retrieve a specific time slot by ID.
/// </summary>
public class GetTimeSlotQuery : IRequest<TimeSlotDto>
{
    /// <summary>
    /// The time slot ID.
    /// </summary>
    public Guid SlotId { get; set; }

    public GetTimeSlotQuery(Guid slotId)
    {
        SlotId = slotId;
    }
}
