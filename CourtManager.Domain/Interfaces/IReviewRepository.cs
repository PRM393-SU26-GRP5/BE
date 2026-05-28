using CourtManager.Domain.Entities;

namespace CourtManager.Domain.Interfaces;

/// <summary>
/// Repository interface for Notification entity operations.
/// </summary>
public interface IReviewRepository : IRepository<Review>
{
    /// <summary>
    /// Gets reviews for a specific football field with pagination.
    /// </summary>
    Task<IEnumerable<Review>> GetReviewsByFieldIdAsync(
        Guid fieldId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Review>> GetReviewsByVenueIdAsync(
        Guid venueId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific user's review for a field (if exists).
    /// </summary>
    Task<Review?> GetUserReviewAsync(
        Guid userId,
        Guid fieldId,
        CancellationToken cancellationToken = default);

    Task<Review?> GetUserReviewForBookingAsync(
        Guid userId,
        Guid bookingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates the average rating for a field.
    /// </summary>
    Task<decimal> GetAverageRatingAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default);

    Task<decimal> GetAverageRatingByVenueIdAsync(
        Guid venueId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total review count for a field.
    /// </summary>
    Task<int> GetReviewCountAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default);
}
