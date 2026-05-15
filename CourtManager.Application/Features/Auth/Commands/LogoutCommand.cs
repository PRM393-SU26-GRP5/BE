using MediatR;
using CourtManager.Application.DTOs;

namespace CourtManager.Application.Features.Auth.Commands;

public class LogoutCommand : IRequest<AuthResponseDto>
{
    public Guid UserId { get; set; }
}
