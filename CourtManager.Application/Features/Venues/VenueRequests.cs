using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Venues;

public record GetVenuesQuery(VenueQueryDto Query) : IRequest<IEnumerable<VenueDto>>;
public record GetVenueByIdQuery(Guid VenueId) : IRequest<VenueDto>;
public record GetNearbyVenuesQuery(decimal Lat, decimal Lng, decimal RadiusKm, int PageNumber, int PageSize) : IRequest<IEnumerable<VenueDto>>;
public record GetOwnerVenuesQuery(Guid OwnerId) : IRequest<IEnumerable<VenueDto>>;
public record CreateVenueCommand(Guid OwnerId, CreateVenueDto Venue) : IRequest<VenueDto>;
public record UpdateVenueCommand(Guid VenueId, Guid OwnerId, UpdateVenueDto Venue) : IRequest<VenueDto>;
public record DeleteVenueCommand(Guid VenueId, Guid OwnerId) : IRequest<bool>;

public class GetVenuesQueryHandler : IRequestHandler<GetVenuesQuery, IEnumerable<VenueDto>>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public GetVenuesQueryHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VenueDto>> Handle(GetVenuesQuery request, CancellationToken cancellationToken)
    {
        FieldType? fieldType = null;
        if (!string.IsNullOrWhiteSpace(request.Query.FieldType))
        {
            if (Enum.TryParse<FieldType>(request.Query.FieldType, true, out var parsed))
            {
                fieldType = parsed;
            }
            else if (int.TryParse(request.Query.FieldType, out var size))
            {
                fieldType = size switch
                {
                    5 => FieldType.FiveASide,
                    7 => FieldType.SevenASide,
                    11 => FieldType.ElevenASide,
                    _ => null
                };
            }
        }

        var venues = await _venueRepository.SearchAsync(
            request.Query.Q,
            fieldType,
            request.Query.MinPrice,
            request.Query.MaxPrice,
            request.Query.MinRating,
            request.Query.Amenity,
            request.Query.SortBy,
            request.Query.Lat,
            request.Query.Lng,
            request.Query.PageNumber,
            request.Query.PageSize,
            cancellationToken);

        return _mapper.Map<IEnumerable<VenueDto>>(venues);
    }
}

public class GetVenueByIdQueryHandler : IRequestHandler<GetVenueByIdQuery, VenueDto>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public GetVenueByIdQueryHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<VenueDto> Handle(GetVenueByIdQuery request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetDetailsAsync(request.VenueId, cancellationToken);
        if (venue == null)
        {
            throw new NotFoundException(nameof(Venue), request.VenueId);
        }

        return _mapper.Map<VenueDto>(venue);
    }
}

public class GetNearbyVenuesQueryHandler : IRequestHandler<GetNearbyVenuesQuery, IEnumerable<VenueDto>>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public GetNearbyVenuesQueryHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VenueDto>> Handle(GetNearbyVenuesQuery request, CancellationToken cancellationToken)
    {
        var venues = await _venueRepository.GetNearbyAsync(request.Lat, request.Lng, request.RadiusKm, request.PageNumber, request.PageSize, cancellationToken);
        return _mapper.Map<IEnumerable<VenueDto>>(venues);
    }
}

public class GetOwnerVenuesQueryHandler : IRequestHandler<GetOwnerVenuesQuery, IEnumerable<VenueDto>>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public GetOwnerVenuesQueryHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VenueDto>> Handle(GetOwnerVenuesQuery request, CancellationToken cancellationToken)
    {
        var venues = await _venueRepository.GetByOwnerAsync(request.OwnerId, cancellationToken);
        return _mapper.Map<IEnumerable<VenueDto>>(venues);
    }
}

public class CreateVenueCommandHandler : IRequestHandler<CreateVenueCommand, VenueDto>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public CreateVenueCommandHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<VenueDto> Handle(CreateVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = new Venue
        {
            VenueId = Guid.NewGuid(),
            OwnerId = request.OwnerId,
            VenueName = request.Venue.VenueName.Trim(),
            Address = request.Venue.Address.Trim(),
            Latitude = request.Venue.Latitude,
            Longitude = request.Venue.Longitude,
            Description = request.Venue.Description,
            OpeningHours = request.Venue.OpeningHours,
            PhoneContact = request.Venue.PhoneContact,
            IsActive = request.Venue.IsActive,
            CreatedAt = DateTime.UtcNow,
            VenueImages = request.Venue.ImageUrls.Select((url, index) => new VenueImage
            {
                ImageId = Guid.NewGuid(),
                ImageUrl = url,
                IsPrimary = index == 0
            }).ToList()
        };

        await _venueRepository.AddAsync(venue, cancellationToken);
        await _venueRepository.SaveChangesAsync(cancellationToken);

        var created = await _venueRepository.GetDetailsAsync(venue.VenueId, cancellationToken) ?? venue;
        return _mapper.Map<VenueDto>(created);
    }
}

public class UpdateVenueCommandHandler : IRequestHandler<UpdateVenueCommand, VenueDto>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public UpdateVenueCommandHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<VenueDto> Handle(UpdateVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetDetailsAsync(request.VenueId, cancellationToken);
        if (venue == null)
        {
            throw new NotFoundException(nameof(Venue), request.VenueId);
        }

        if (venue.OwnerId != request.OwnerId)
        {
            throw new ValidationException("Only the venue owner can update this venue.");
        }

        venue.VenueName = request.Venue.VenueName.Trim();
        venue.Address = request.Venue.Address.Trim();
        venue.Latitude = request.Venue.Latitude;
        venue.Longitude = request.Venue.Longitude;
        venue.Description = request.Venue.Description;
        venue.OpeningHours = request.Venue.OpeningHours;
        venue.PhoneContact = request.Venue.PhoneContact;
        venue.IsActive = request.Venue.IsActive;
        venue.UpdatedAt = DateTime.UtcNow;

        await _venueRepository.UpdateAsync(venue, cancellationToken);
        await _venueRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<VenueDto>(venue);
    }
}

public class DeleteVenueCommandHandler : IRequestHandler<DeleteVenueCommand, bool>
{
    private readonly IVenueRepository _venueRepository;

    public DeleteVenueCommandHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<bool> Handle(DeleteVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetDetailsAsync(request.VenueId, cancellationToken);
        if (venue == null)
        {
            throw new NotFoundException(nameof(Venue), request.VenueId);
        }

        if (venue.OwnerId != request.OwnerId)
        {
            throw new ValidationException("Only the venue owner can delete this venue.");
        }

        await _venueRepository.DeleteAsync(venue, cancellationToken);
        await _venueRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
