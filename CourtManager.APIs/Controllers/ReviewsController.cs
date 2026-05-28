using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Reviews;
using CourtManager.Application.Features.Reviews.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// API endpoint for managing reviews and ratings.
/// Provides CRUD operations for field reviews and ratings.
/// </summary>
[ApiController]
[Route("api/v1/reviews")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IMediator mediator, ILogger<ReviewsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all reviews for a specific field.
    /// </summary>
    /// <param name="fieldId">The field ID</param>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 10)</param>
    /// <returns>Paginated list of reviews</returns>
    [NonAction]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByField(Guid fieldId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching reviews for field {FieldId}", fieldId);
        var result = await _mediator.Send(new GetReviewsByFieldQuery(fieldId, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("venue/{venueId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByVenue(Guid venueId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetReviewsByVenueQuery(venueId, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific review by ID.
    /// </summary>
    /// <param name="id">The review ID</param>
    /// <returns>Review details</returns>
    [NonAction]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> GetReviewById(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching review {ReviewId}", id);
        var result = await _mediator.Send(new GetReviewByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets average rating for a field.
    /// </summary>
    /// <param name="fieldId">The field ID</param>
    /// <returns>Average rating</returns>
    [NonAction]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetAverageRating(Guid fieldId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching average rating for field {FieldId}", fieldId);
        var reviews = await _mediator.Send(new GetReviewsByFieldQuery(fieldId, 1, int.MaxValue), cancellationToken);
        var list = reviews.ToList();
        return Ok(new { fieldId, averageRating = list.Count == 0 ? 0 : list.Average(r => r.Rating), reviewCount = list.Count });
    }

    [NonAction]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetAverageVenueRating(Guid venueId, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAverageVenueRatingQuery(venueId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets all reviews by the current user.
    /// </summary>
    /// <returns>List of user's reviews</returns>
    [NonAction]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetMyReviews(CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Fetching reviews for current user {UserId}", userId);
        var result = await _mediator.Send(new GetMyReviewsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new review for a field.
    /// </summary>
    /// <param name="review">The review creation data</param>
    /// <returns>Created review</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] ReviewDto review, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Creating review for field {FieldId} by user {UserId}", review.FieldId, userId);
        var result = await _mediator.Send(new CreateReviewCommand(GetCurrentUserId(), review), cancellationToken);
        return Created($"/api/v1/reviews/{result.ReviewId}", result);
    }

    /// <summary>
    /// Updates an existing review (Owner only).
    /// </summary>
    /// <param name="id">The review ID</param>
    /// <param name="review">The updated review data</param>
    /// <returns>Updated review</returns>
    [NonAction]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ReviewDto>> UpdateReview(Guid id, [FromBody] ReviewDto review, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating review {ReviewId}", id);
        var result = await _mediator.Send(new UpdateReviewCommand(GetCurrentUserId(), id, review), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a review (soft delete - Owner/Admin only).
    /// </summary>
    /// <param name="id">The review ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [NonAction]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteReview(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting review {ReviewId}", id);
        var command = new DeleteReviewCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        _logger.LogInformation("Review {ReviewId} deleted successfully (soft delete)", id);
        return Ok(new { success = result, message = "Review deleted successfully" });
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }
}
