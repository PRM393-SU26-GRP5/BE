using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Auth.Queries;

public class GetMeQuery : IRequest<UserDto>
{
    // Empty because UserId will be resolved from ICurrentUserService
}
