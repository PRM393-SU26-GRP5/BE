using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Entities;

namespace CourtManager.Application.Mappings;

/// <summary>
/// AutoMapper profile for entity to DTO mappings.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>().ReverseMap();

        // FootballField mappings
        CreateMap<FootballField, FootballFieldDto>().ReverseMap();

        // Booking mappings
        CreateMap<Booking, BookingDto>().ReverseMap();
        CreateMap<Booking, CreateBookingDto>().ReverseMap();

        // Payment mappings
        CreateMap<Payment, PaymentDto>().ReverseMap();
    }
}
