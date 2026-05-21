using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Query to retrieve all bookings for a specific football field.
/// </summary>
public class GetFieldBookingsQuery : IRequest<IEnumerable<BookingDto>>
{
    /// <summary>
    /// The field ID to fetch bookings for.
    /// </summary>
    public Guid FieldId { get; set; }

    public GetFieldBookingsQuery(Guid fieldId)
    {
        FieldId = fieldId;
    }
}
