using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Handler for CancelBookingCommand.
/// Implements the business logic for cancelling a booking and unlocking reserved time slots.
/// </summary>
public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, bool>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;

    public CancelBookingCommandHandler(
        IBookingRepository bookingRepository,
        ITimeSlotRepository timeSlotRepository)
    {
        _bookingRepository = bookingRepository;
        _timeSlotRepository = timeSlotRepository;
    }

    /// <summary>
    /// Handles the CancelBookingCommand.
    /// Verifies booking exists, is cancellable, updates status to Cancelled,
    /// and reverts associated time slots to Available.
    /// </summary>
    public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        // Fetch the booking
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
            throw new NotFoundException(nameof(Booking), request.BookingId);

        // Verify booking is in a cancellable status (Pending or Confirmed)
        if (booking.BookingStatus != "Pending" && booking.BookingStatus != "Confirmed")
            throw new ValidationException(
                $"Cannot cancel booking. Current status is '{booking.BookingStatus}'. Only 'Pending' or 'Confirmed' bookings can be cancelled.");

        // Update booking status to Cancelled and store cancellation reason
        booking.BookingStatus = "Cancelled";
        if (!string.IsNullOrEmpty(request.CancellationReason))
        {
            booking.Note = $"Cancelled: {request.CancellationReason}";
        }
        booking.UpdatedAt = DateTime.UtcNow;

        // Revert time slots to Available status
        if (booking.BookingItems != null && booking.BookingItems.Any())
        {
            var slotIds = booking.BookingItems.Select(bi => bi.SlotId).ToList();
            await _timeSlotRepository.BatchUpdateSlotStatusAsync(slotIds, "Available", cancellationToken);
        }

        // Save changes
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
        await _bookingRepository.SaveChangesAsync(cancellationToken);
        await _timeSlotRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
