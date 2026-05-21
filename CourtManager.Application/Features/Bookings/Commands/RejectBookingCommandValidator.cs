using FluentValidation;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Validator for RejectBookingCommand.
/// Validates business rules and data constraints.
/// </summary>
public class RejectBookingCommandValidator : AbstractValidator<RejectBookingCommand>
{
    public RejectBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Booking ID is required.");

        RuleFor(x => x.RejectionReason)
            .MaximumLength(500)
            .WithMessage("Rejection reason must not exceed 500 characters.");
    }
}
