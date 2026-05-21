using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Query to retrieve all bookings for a specific user.
/// </summary>
public class GetUserBookingsQuery : IRequest<IEnumerable<BookingDto>>
{
    /// <summary>
    /// The user ID to fetch bookings for.
    /// </summary>
    public Guid UserId { get; set; }

    public GetUserBookingsQuery(Guid userId)
    {
        UserId = userId;
    }
}
