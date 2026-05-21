using CourtManager.Application.DTOs;
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
[Route("api/[controller]")]
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
    [HttpGet("field/{fieldId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetReviewsByField(Guid fieldId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Fetching reviews for field {FieldId}", fieldId);
        return Ok(new { message = "Get reviews by field endpoint - implementation pending" });
    }

    /// <summary>
    /// Gets a specific review by ID.
    /// </summary>
    /// <param name="id">The review ID</param>
    /// <returns>Review details</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetReviewById(Guid id)
    {
        _logger.LogInformation("Fetching review {ReviewId}", id);
        return Ok(new { message = "Get review by ID endpoint - implementation pending" });
    }

    /// <summary>
    /// Gets average rating for a field.
    /// </summary>
    /// <param name="fieldId">The field ID</param>
    /// <returns>Average rating</returns>
    [HttpGet("field/{fieldId}/average-rating")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetAverageRating(Guid fieldId)
    {
        _logger.LogInformation("Fetching average rating for field {FieldId}", fieldId);
        return Ok(new { message = "Get average rating endpoint - implementation pending" });
    }

    /// <summary>
    /// Gets all reviews by the current user.
    /// </summary>
    /// <returns>List of user's reviews</returns>
    [HttpGet("my-reviews")]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
    public IActionResult GetMyReviews()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Fetching reviews for current user {UserId}", userId);
        return Ok(new { message = "Get my reviews endpoint - implementation pending" });
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
    public IActionResult CreateReview([FromBody] ReviewDto review)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Creating review for field {FieldId} by user {UserId}", review.FieldId, userId);
        return Ok(new { message = "Create review endpoint - implementation pending" });
    }

    /// <summary>
    /// Updates an existing review (Owner only).
    /// </summary>
    /// <param name="id">The review ID</param>
    /// <param name="review">The updated review data</param>
    /// <returns>Updated review</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult UpdateReview(Guid id, [FromBody] ReviewDto review)
    {
        _logger.LogInformation("Updating review {ReviewId}", id);
        return Ok(new { message = "Update review endpoint - implementation pending" });
    }

    /// <summary>
    /// Deletes a review (soft delete - Owner/Admin only).
    /// </summary>
    /// <param name="id">The review ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
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
}
