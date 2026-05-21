using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Query to retrieve available time slots for a specific field and date.
/// </summary>
public class GetAvailableSlotsQuery : IRequest<IEnumerable<TimeSlotDto>>
{
    /// <summary>
    /// The field ID to fetch available slots for.
    /// </summary>
    public Guid FieldId { get; set; }

    /// <summary>
    /// The date to fetch slots for.
    /// </summary>
    public DateTime Date { get; set; }

    public GetAvailableSlotsQuery(Guid fieldId, DateTime date)
    {
        FieldId = fieldId;
        Date = date;
    }
}
