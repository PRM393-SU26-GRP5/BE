using FluentValidation;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Validator for LockTimeSlotCommand.
/// Validates business rules and data constraints.
/// </summary>
public class LockTimeSlotCommandValidator : AbstractValidator<LockTimeSlotCommand>
{
    public LockTimeSlotCommandValidator()
    {
        RuleFor(x => x.SlotId)
            .NotEmpty()
            .WithMessage("Slot ID is required.");

        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Booking ID is required.");
    }
}
