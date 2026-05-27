using MediatR;
using CourtManager.Application.DTOs;

namespace CourtManager.Application.Features.Auth.Commands;

public class LogoutCommand : IRequest<AuthResponseDto>
{
    // Empty because UserId will be resolved from ICurrentUserService
}
