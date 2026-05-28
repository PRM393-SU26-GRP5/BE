using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Bookings.Commands;
using CourtManager.Application.Features.Bookings.Queries;
using CourtManager.Application.Features.Discounts;
using CourtManager.Application.Features.Fields;
using CourtManager.Application.Features.Owner;
using CourtManager.Application.Features.TimeSlots;
using CourtManager.Application.Features.TimeSlots.Commands;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using CourtManager.Infrastructure;
using CourtManager.Application.Features.Venues;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/v1/owner")]
[Authorize(Roles = "Manager,Admin")]
public class OwnerController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IVenueRepository _venueRepository;
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly ApplicationDbContext _dbContext;

    public OwnerController(
        IMediator mediator,
        IVenueRepository venueRepository,
        IFootballFieldRepository fieldRepository,
        ITimeSlotRepository timeSlotRepository,
        IBookingRepository bookingRepository,
        IDiscountRepository discountRepository,
        ApplicationDbContext dbContext)
    {
        _mediator = mediator;
        _venueRepository = venueRepository;
        _fieldRepository = fieldRepository;
        _timeSlotRepository = timeSlotRepository;
        _bookingRepository = bookingRepository;
        _discountRepository = discountRepository;
        _dbContext = dbContext;
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(OwnerStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OwnerStatsDto>> GetStats(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOwnerStatsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string groupBy = "day", CancellationToken cancellationToken = default)
    {
        var bookings = await _bookingRepository.GetBookingsForOwnerAsync(GetCurrentUserId(), cancellationToken);
        var payments = bookings
            .SelectMany(b => b.Payments.Select(p => new
            {
                Payment = p,
                Booking = b,
                VenueId = b.BookingItems.Select(i => i.Slot?.Field?.VenueId).FirstOrDefault(),
                VenueName = b.BookingItems.Select(i => i.Slot?.Field?.Venue?.VenueName).FirstOrDefault()
            }))
            .Where(x => x.Payment.PaymentStatus == PaymentStatus.Success && x.Payment.PaidAt.HasValue)
            .Where(x => !from.HasValue || x.Payment.PaidAt!.Value.Date >= from.Value.Date)
            .Where(x => !to.HasValue || x.Payment.PaidAt!.Value.Date <= to.Value.Date)
            .ToList();

        var data = groupBy.Trim().ToLowerInvariant() switch
        {
            "month" => payments
                .GroupBy(x => x.Payment.PaidAt!.Value.ToString("yyyy-MM"))
                .Select(g => new { key = g.Key, revenue = g.Sum(x => x.Payment.Amount), payments = g.Count() }),
            "venue" => payments
                .GroupBy(x => x.VenueName ?? x.VenueId?.ToString() ?? "unknown")
                .Select(g => new { key = g.Key, revenue = g.Sum(x => x.Payment.Amount), payments = g.Count() }),
            _ => payments
                .GroupBy(x => x.Payment.PaidAt!.Value.Date.ToString("yyyy-MM-dd"))
                .Select(g => new { key = g.Key, revenue = g.Sum(x => x.Payment.Amount), payments = g.Count() })
        };

        return Ok(data);
    }

    [HttpGet("venues")]
    [ProducesResponseType(typeof(IEnumerable<VenueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VenueDto>>> GetVenues(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOwnerVenuesQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpPost("venues")]
    [ProducesResponseType(typeof(VenueDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<VenueDto>> CreateVenue([FromBody] CreateVenueDto venue, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateVenueCommand(GetCurrentUserId(), venue), cancellationToken);
        return Created($"/api/v1/venues/{result.VenueId}", result);
    }

    [HttpPut("venues/{id:guid}")]
    public async Task<ActionResult<VenueDto>> UpdateVenue(Guid id, [FromBody] UpdateVenueDto venue, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateVenueCommand(id, GetCurrentUserId(), venue), cancellationToken);
        return Ok(result);
    }

    [HttpPut("venues/{id:guid}/status")]
    public async Task<IActionResult> UpdateVenueStatus(Guid id, [FromBody] UpdateStatusDto request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetDetailsAsync(id, cancellationToken);
        if (venue == null) return NotFound();
        if (venue.OwnerId != GetCurrentUserId()) return Forbid();

        venue.IsActive = request.IsActive;
        venue.UpdatedAt = DateTime.UtcNow;
        await _venueRepository.UpdateAsync(venue, cancellationToken);
        await _venueRepository.SaveChangesAsync(cancellationToken);
        return Ok(new { venueId = id, isActive = venue.IsActive });
    }

    [HttpPost("venues/{id:guid}/images")]
    public async Task<IActionResult> AddVenueImage(Guid id, [FromBody] VenueImageRequestDto request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetDetailsAsync(id, cancellationToken);
        if (venue == null) return NotFound();
        if (venue.OwnerId != GetCurrentUserId()) return Forbid();

        var image = new VenueImage
        {
            ImageId = Guid.NewGuid(),
            VenueId = id,
            ImageUrl = request.ImageUrl,
            IsPrimary = request.IsPrimary
        };

        await _dbContext.VenueImages.AddAsync(image, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Created($"/api/v1/venues/{id}/images", image);
    }

    [HttpDelete("venues/{id:guid}/images/{imageId:guid}")]
    public async Task<IActionResult> DeleteVenueImage(Guid id, Guid imageId, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetDetailsAsync(id, cancellationToken);
        if (venue == null) return NotFound();
        if (venue.OwnerId != GetCurrentUserId()) return Forbid();

        var image = await _dbContext.VenueImages.FindAsync(new object[] { imageId }, cancellationToken);
        if (image == null || image.VenueId != id) return NotFound();

        image.IsDeleted = true;
        image.DeletedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(new { success = true });
    }

    [HttpPost("venues/{id:guid}/amenities")]
    public async Task<IActionResult> AddVenueAmenities(Guid id, [FromBody] VenueAmenityRequestDto request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetDetailsAsync(id, cancellationToken);
        if (venue == null) return NotFound();
        if (venue.OwnerId != GetCurrentUserId()) return Forbid();

        var amenityIds = request.AmenityIds.Count > 0
            ? request.AmenityIds
            : request.AmenityId.HasValue
                ? new List<Guid> { request.AmenityId.Value }
                : new List<Guid>();
        foreach (var amenityId in amenityIds.Distinct())
        {
            var exists = await _dbContext.Amenities.FindAsync(new object[] { amenityId }, cancellationToken);
            if (exists == null) return NotFound(new { message = $"Amenity {amenityId} was not found" });

            if (!_dbContext.VenueAmenities.Any(va => va.VenueId == id && va.AmenityId == amenityId))
            {
                await _dbContext.VenueAmenities.AddAsync(new VenueAmenity { VenueId = id, AmenityId = amenityId }, cancellationToken);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        var updated = await _venueRepository.GetDetailsAsync(id, cancellationToken);
        return Ok(updated?.VenueAmenities.Select(va => va.Amenity?.Name).Where(name => name != null));
    }

    [HttpDelete("venues/{id:guid}/amenities/{amenityId:guid}")]
    public async Task<IActionResult> DeleteVenueAmenity(Guid id, Guid amenityId, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetDetailsAsync(id, cancellationToken);
        if (venue == null) return NotFound();
        if (venue.OwnerId != GetCurrentUserId()) return Forbid();

        var venueAmenity = await _dbContext.VenueAmenities.FindAsync(new object[] { id, amenityId }, cancellationToken);
        if (venueAmenity == null) return NotFound();
        _dbContext.VenueAmenities.Remove(venueAmenity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(new { success = true });
    }

    [HttpGet("venues/{venueId}/fields")]
    [ProducesResponseType(typeof(IEnumerable<FootballFieldDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FootballFieldDto>>> GetFields(Guid venueId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFieldsByVenueQuery(venueId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("venues/{venueId}/fields")]
    [ProducesResponseType(typeof(FootballFieldDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<FootballFieldDto>> CreateField(Guid venueId, [FromBody] FootballFieldDto field, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateFieldCommand(GetCurrentUserId(), venueId, field), cancellationToken);
        return Created($"/api/v1/fields/{result.Id}", result);
    }

    [HttpPut("fields/{id:guid}")]
    public async Task<ActionResult<FootballFieldDto>> UpdateField(Guid id, [FromBody] FootballFieldDto field, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateFieldCommand(GetCurrentUserId(), id, field), cancellationToken);
        return Ok(result);
    }

    [HttpPut("fields/{id:guid}/status")]
    public async Task<IActionResult> UpdateFieldStatus(Guid id, [FromBody] UpdateStatusDto request, CancellationToken cancellationToken)
    {
        var field = await _fieldRepository.GetByIdAsync(id, cancellationToken);
        if (field == null) return NotFound();
        if (field.Venue?.OwnerId != GetCurrentUserId()) return Forbid();

        field.IsActive = request.IsActive;
        field.UpdatedAt = DateTime.UtcNow;
        await _fieldRepository.UpdateAsync(field, cancellationToken);
        await _fieldRepository.SaveChangesAsync(cancellationToken);
        return Ok(new { fieldId = id, isActive = field.IsActive });
    }

    [HttpPost("fields/{id:guid}/slots/bulk")]
    public async Task<IActionResult> BulkCreateSlots(Guid id, [FromBody] BulkCreateSlotsDto request, CancellationToken cancellationToken)
    {
        var field = await _fieldRepository.GetByIdAsync(id, cancellationToken);
        if (field == null) return NotFound();
        if (field.Venue?.OwnerId != GetCurrentUserId()) return Forbid();

        var startTime = TimeSpan.Parse(request.StartTime);
        var endTime = TimeSpan.Parse(request.EndTime);
        var created = 0;

        for (var date = request.FromDate.Date; date <= request.ToDate.Date; date = date.AddDays(1))
        {
            for (var slotStart = startTime; slotStart.Add(TimeSpan.FromMinutes(request.SlotDurationMinutes)) <= endTime; slotStart = slotStart.Add(TimeSpan.FromMinutes(request.SlotDurationMinutes)))
            {
                var start = DateTime.SpecifyKind(date.Add(slotStart), DateTimeKind.Utc);
                var end = start.AddMinutes(request.SlotDurationMinutes);
                await _timeSlotRepository.AddAsync(new TimeSlot
                {
                    SlotId = Guid.NewGuid(),
                    FieldId = id,
                    StartTime = start,
                    EndTime = end,
                    Price = request.Price,
                    SlotStatus = SlotStatus.Available,
                    CreatedAt = DateTime.UtcNow
                }, cancellationToken);
                created++;
            }
        }

        await _timeSlotRepository.SaveChangesAsync(cancellationToken);
        return Ok(new { createdSlots = created });
    }

    [HttpPut("slots/{id:guid}")]
    public async Task<ActionResult<TimeSlotDto>> UpdateSlot(Guid id, [FromBody] TimeSlotDto slot, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateTimeSlotCommand(GetCurrentUserId(), id, slot), cancellationToken);
        return Ok(result);
    }

    [HttpPut("slots/{id:guid}/status")]
    public async Task<IActionResult> UpdateSlotStatus(Guid id, [FromBody] UpdateSlotStatusDto request, CancellationToken cancellationToken)
    {
        var slot = await _timeSlotRepository.GetByIdAsync(id, cancellationToken);
        if (slot == null) return NotFound();
        if (slot.Field?.Venue?.OwnerId != GetCurrentUserId()) return Forbid();
        if (!Enum.TryParse<SlotStatus>(request.SlotStatus, true, out var status)) return BadRequest(new { message = "Invalid slotStatus" });

        slot.SlotStatus = status;
        slot.UpdatedAt = DateTime.UtcNow;
        await _timeSlotRepository.UpdateAsync(slot, cancellationToken);
        await _timeSlotRepository.SaveChangesAsync(cancellationToken);
        return Ok(new { slotId = id, slotStatus = slot.SlotStatus.ToString() });
    }

    [HttpDelete("slots/{id:guid}")]
    public async Task<IActionResult> DeleteSlot(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteTimeSlotCommand(id), cancellationToken);
        return Ok(new { success = result });
    }

    [HttpGet("bookings/pending")]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetPendingBookings(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOwnerPendingBookingsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpGet("bookings")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOwnerBookingsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpGet("bookings/{id:guid}")]
    public async Task<ActionResult<BookingDto>> GetBooking(Guid id, CancellationToken cancellationToken)
    {
        var bookings = await _mediator.Send(new GetOwnerBookingsQuery(GetCurrentUserId()), cancellationToken);
        var booking = bookings.FirstOrDefault(b => b.Id == id);
        return booking == null ? NotFound() : Ok(booking);
    }

    [HttpPut("bookings/{id:guid}/accept")]
    public async Task<IActionResult> AcceptBooking(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AcceptBookingCommand(id, GetCurrentUserId()), cancellationToken);
        return Ok(new { success = result });
    }

    [HttpPut("bookings/{id:guid}/reject")]
    public async Task<IActionResult> RejectBooking(Guid id, [FromQuery] string? rejectionReason, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RejectBookingCommand(id, rejectionReason, GetCurrentUserId()), cancellationToken);
        return Ok(new { success = result });
    }

    [HttpPut("bookings/{id:guid}/complete")]
    public async Task<IActionResult> CompleteBooking(Guid id, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(id, cancellationToken);
        if (booking == null) return NotFound();
        if (!booking.BookingItems.Any(i => i.Slot?.Field?.Venue?.OwnerId == GetCurrentUserId())) return Forbid();

        booking.BookingStatus = BookingStatus.Completed;
        booking.UpdatedAt = DateTime.UtcNow;
        foreach (var item in booking.BookingItems)
        {
            if (item.Slot != null)
            {
                item.Slot.SlotStatus = SlotStatus.Booked;
                item.Slot.LockedUntil = null;
            }
        }

        await _bookingRepository.UpdateAsync(booking, cancellationToken);
        await _bookingRepository.SaveChangesAsync(cancellationToken);
        return Ok(new { bookingId = id, bookingStatus = booking.BookingStatus.ToString() });
    }

    [HttpGet("discounts")]
    [ProducesResponseType(typeof(IEnumerable<DiscountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DiscountDto>>> GetDiscounts(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOwnerDiscountsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpPost("discounts")]
    [ProducesResponseType(typeof(DiscountDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<DiscountDto>> CreateDiscount([FromBody] DiscountDto discount, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateDiscountCommand(GetCurrentUserId(), discount), cancellationToken);
        return Created($"/api/v1/owner/discounts/{result.DiscountId}", result);
    }

    [HttpPut("discounts/{id:guid}")]
    public async Task<ActionResult<DiscountDto>> UpdateDiscount(Guid id, [FromBody] DiscountDto discount, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateDiscountCommand(id, GetCurrentUserId(), discount), cancellationToken);
        return Ok(result);
    }

    [HttpPut("discounts/{id:guid}/status")]
    public async Task<IActionResult> UpdateDiscountStatus(Guid id, [FromBody] UpdateStatusDto request, CancellationToken cancellationToken)
    {
        var discount = await _discountRepository.GetByIdAsync(id, cancellationToken);
        if (discount == null) return NotFound();
        if (discount.OwnerId != GetCurrentUserId()) return Forbid();

        discount.IsActive = request.IsActive;
        await _discountRepository.UpdateAsync(discount, cancellationToken);
        await _discountRepository.SaveChangesAsync(cancellationToken);
        return Ok(new { discountId = id, isActive = discount.IsActive });
    }

    [HttpDelete("discounts/{id:guid}")]
    public async Task<IActionResult> DeleteDiscount(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteDiscountCommand(id, GetCurrentUserId()), cancellationToken);
        return Ok(new { success = result });
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }
}

public class UpdateStatusDto
{
    public bool IsActive { get; set; }
}

public class UpdateSlotStatusDto
{
    public string SlotStatus { get; set; } = string.Empty;
}

public class VenueImageRequestDto
{
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class VenueAmenityRequestDto
{
    public Guid? AmenityId { get; set; }
    public List<Guid> AmenityIds { get; set; } = [];
}

public class BulkCreateSlotsDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string StartTime { get; set; } = "06:00";
    public string EndTime { get; set; } = "23:00";
    public int SlotDurationMinutes { get; set; } = 60;
    public decimal Price { get; set; }
}
