using FluentValidation;

namespace CourtManager.Application.Features.TimeSlots.Commands;

/// <summary>
/// Validator for DeleteTimeSlotCommand.
/// </summary>
public class DeleteTimeSlotCommandValidator : AbstractValidator<DeleteTimeSlotCommand>
{
    public DeleteTimeSlotCommandValidator()
    {
        RuleFor(x => x.SlotId)
            .NotEmpty().WithMessage("Slot ID is required");
    }
}
