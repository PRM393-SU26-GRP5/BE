using FluentValidation;

namespace CourtManager.Application.Features.Chats.Commands;

/// <summary>
/// Validator for DeleteChatRoomCommand.
/// </summary>
public class DeleteChatRoomCommandValidator : AbstractValidator<DeleteChatRoomCommand>
{
    public DeleteChatRoomCommandValidator()
    {
        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage("Chat room ID is required");
    }
}
