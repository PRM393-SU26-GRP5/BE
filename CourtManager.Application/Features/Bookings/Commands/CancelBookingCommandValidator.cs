using FluentValidation;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Validator for CancelBookingCommand.
/// Validates business rules and data constraints.
/// </summary>
public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Booking ID is required.");

        RuleFor(x => x.CancellationReason)
            .MaximumLength(500)
            .WithMessage("Cancellation reason must not exceed 500 characters.");
    }
}
