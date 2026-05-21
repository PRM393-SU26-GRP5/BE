using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Handler for GetUserBookingsQuery.
/// Retrieves all bookings for a specific user.
/// </summary>
public class GetUserBookingsQueryHandler : IRequestHandler<GetUserBookingsQuery, IEnumerable<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public GetUserBookingsQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookingDto>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetBookingsByUserIdAsync(request.UserId, cancellationToken);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }
}
