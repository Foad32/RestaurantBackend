using MediatR;
using Restaurant.Core.Domain.DTOs;

namespace Restaurant.Core.ApplicationService.Features.RegisterUser.Commands.Request;

public record RegisterUserCommand : IRequest<int>
{
    public required RegisterUserDTO RegisterUserDTO { get; set; }
}

