using CourtManager.Application.DTOs;
using CourtManager.Domain.Enums;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

public class GetVenuesQuery : IRequest<PagedResult<VenueDto>>
{
    public string? Q { get; set; }
    public FieldType? FieldType { get; set; }
    public List<Guid>? AmenityIds { get; set; }
    public double? MinRating { get; set; }
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public string? Sort { get; set; }
    
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
