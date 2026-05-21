using MediatR;

namespace CourtManager.Application.Features.Reviews.Commands;

/// <summary>
/// Command to delete a review (soft delete).
/// </summary>
public class DeleteReviewCommand : IRequest<bool>
{
    /// <summary>
    /// The ID of the review to delete.
    /// </summary>
    public Guid ReviewId { get; set; }

    public DeleteReviewCommand(Guid reviewId)
    {
        ReviewId = reviewId;
    }
}
