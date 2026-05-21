using FluentValidation;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Validator for AcceptBookingCommand.
/// Validates business rules and data constraints.
/// </summary>
public class AcceptBookingCommandValidator : AbstractValidator<AcceptBookingCommand>
{
    public AcceptBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Booking ID is required.");
    }
}
