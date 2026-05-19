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
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public CreateBookingCommandHandler(
        IUserRepository userRepository,
        IFootballFieldRepository fieldRepository,
        IBookingRepository bookingRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _fieldRepository = fieldRepository;
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the CreateBookingCommand.
    /// Validates user and field existence, checks field availability,
    /// calculates total price, and creates the booking.
    /// </summary>
    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), request.UserId);

        // Validate football field exists
        var field = await _fieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
        if (field == null)
            throw new NotFoundException(nameof(FootballField), request.FieldId);

        // Check field availability
        var isAvailable = await _bookingRepository.IsCourtAvailableAsync(
            request.FieldId, request.StartTime, request.EndTime, cancellationToken);

        if (!isAvailable)
            throw new ValidationException(
                $"Field '{field.FieldName}' is not available for the specified time period.");

        // Calculate total price (hours * price per hour)
        var durationInHours = (decimal)(request.EndTime - request.StartTime).TotalHours;
        var totalPrice = durationInHours * field.PricePerHour;

        // Create booking entity
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            FieldId = request.FieldId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TotalPrice = totalPrice,
            BookingStatus = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        // Save booking
        var createdBooking = await _bookingRepository.AddAsync(booking, cancellationToken);
        await _bookingRepository.SaveChangesAsync(cancellationToken);

        // Map to DTO and return
        return _mapper.Map<BookingDto>(createdBooking);
    }
}
