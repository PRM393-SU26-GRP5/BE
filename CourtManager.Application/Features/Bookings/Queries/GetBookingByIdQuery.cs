using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Query to retrieve a booking by ID.
/// </summary>
public class GetBookingByIdQuery : IRequest<BookingDto>
{
    public Guid BookingId { get; set; }

    public GetBookingByIdQuery(Guid bookingId)
    {
        BookingId = bookingId;
    }
}
