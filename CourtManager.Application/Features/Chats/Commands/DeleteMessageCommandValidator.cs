using FluentValidation;

namespace CourtManager.Application.Features.Chats.Commands;

/// <summary>
/// Validator for DeleteMessageCommand.
/// </summary>
public class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
{
    public DeleteMessageCommandValidator()
    {
        RuleFor(x => x.MessageId)
            .NotEmpty().WithMessage("Message ID is required");
    }
}
