using MediatR;
using CourtManager.Application.DTOs;

namespace CourtManager.Application.Features.Auth.Commands;

public class ForgotPasswordCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
}
