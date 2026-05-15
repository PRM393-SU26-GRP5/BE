using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Handler for CreateBookingCommand.
/// Implements the business logic for creating a new booking.
/// </summary>
public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ICourtRepository _courtRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public CreateBookingCommandHandler(
        IUserRepository userRepository,
        ICourtRepository courtRepository,
        IBookingRepository bookingRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _courtRepository = courtRepository;
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the CreateBookingCommand.
    /// Validates user and court existence, checks court availability,
    /// calculates total amount, and creates the booking.
    /// </summary>
    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), request.UserId);

        // Validate court exists
        var court = await _courtRepository.GetByIdAsync(request.CourtId, cancellationToken);
        if (court == null)
            throw new NotFoundException(nameof(Court), request.CourtId);

        // Check court availability
        var isAvailable = await _bookingRepository.IsCourtAvailableAsync(
            request.CourtId, request.StartTime, request.EndTime, cancellationToken);

        if (!isAvailable)
            throw new ValidationException(
                $"Court '{court.Name}' is not available for the specified time period.");

        // Calculate total amount (hours * price per hour)
        var durationInHours = (decimal)(request.EndTime - request.StartTime).TotalHours;
        var totalAmount = durationInHours * court.PricePerHour;

        // Create booking entity
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            CourtId = request.CourtId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TotalAmount = totalAmount,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        // Save booking
        var createdBooking = await _bookingRepository.AddAsync(booking, cancellationToken);
        await _bookingRepository.SaveChangesAsync(cancellationToken);

        // Map to DTO and return
        return _mapper.Map<BookingDto>(createdBooking);
    }
}
