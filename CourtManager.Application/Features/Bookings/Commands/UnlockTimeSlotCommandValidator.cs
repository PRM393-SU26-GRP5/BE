using FluentValidation;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Validator for UnlockTimeSlotCommand.
/// Validates business rules and data constraints.
/// </summary>
public class UnlockTimeSlotCommandValidator : AbstractValidator<UnlockTimeSlotCommand>
{
    public UnlockTimeSlotCommandValidator()
    {
        RuleFor(x => x.SlotId)
            .NotEmpty()
            .WithMessage("Slot ID is required.");

        RuleFor(x => x.UnlockReason)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Unlock reason must not exceed 100 characters.");
    }
}
