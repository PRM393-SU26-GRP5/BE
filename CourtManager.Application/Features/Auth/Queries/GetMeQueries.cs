using MediatR;
using CourtManager.Application.DTOs;

namespace CourtManager.Application.Features.Auth.Queries
{
    public class GetMeQueries : IRequest<UserDto>
    {
        public Guid UserId { get; set; }
    }
}
