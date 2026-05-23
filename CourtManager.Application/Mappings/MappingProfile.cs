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
        CreateMap<Booking, BookingHistoryDto>()
            //.ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field != null ? src.Field.FieldName : null))
            //.ForMember(dest => dest.FieldLocation, opt => opt.MapFrom(src => src.Field != null ? src.Field.Location : null))
            //.ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Field != null && src.Field.Owner != null ? src.Field.Owner.FullName : null))
            .ReverseMap();

        // Payment mappings
        CreateMap<Payment, PaymentDto>().ReverseMap();

        // TimeSlot mappings
        CreateMap<TimeSlot, TimeSlotDto>().ReverseMap();

        // FieldImage mappings
        // CreateMap<FieldImage, FieldImageDto>().ReverseMap();

        // ChatRoom mappings
        CreateMap<ChatRoom, ChatRoomDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : null))
            .ForMember(dest => dest.HostName, opt => opt.MapFrom(src => src.Host != null ? src.Host.FullName : null))
            .ReverseMap();

        // Message mappings
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender != null ? src.Sender.FullName : null))
            .ReverseMap();

        // Notification mappings
        CreateMap<Notification, NotificationDto>().ReverseMap();

        // Review mappings
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : null))
            //.ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field != null ? src.Field.FieldName : null))
            .ReverseMap();
    }
}
