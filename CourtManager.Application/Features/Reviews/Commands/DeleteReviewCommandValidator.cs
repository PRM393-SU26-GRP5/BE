using FluentValidation;

namespace CourtManager.Application.Features.Reviews.Commands;

/// <summary>
/// Validator for DeleteReviewCommand.
/// </summary>
public class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
{
    public DeleteReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId)
            .NotEmpty().WithMessage("Review ID is required");
    }
}
