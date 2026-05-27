using MediatR;
using CourtManager.Application.DTOs;

namespace CourtManager.Application.Features.Auth.Commands;

public class ChangePasswordCommand : IRequest<AuthResponseDto>
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
