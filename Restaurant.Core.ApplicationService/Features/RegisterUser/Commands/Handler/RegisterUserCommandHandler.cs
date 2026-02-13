using MediatR;
using Restaurant.Core.ApplicationService.Extensions;
using Restaurant.Core.ApplicationService.Features.RegisterUser.Commands.Request;
using Restaurant.Core.Contracts.UnitOfWork;
using Restaurant.Core.Domain.DTOs;
using Restaurant.Core.Domain.Models;

namespace Restaurant.Core.ApplicationService.Features.RegisterUser.Commands.Handler;

internal sealed class RegisterUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RegisterUserCommand, int>
{
    public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        RegisterUserDTO req = request.RegisterUserDTO;

        User? isUserPresent = await unitOfWork.Register.GetUserByPhoneNumber(req.PhoneNumber);
        if (isUserPresent != null) throw new Exception("User already exists");

        var (hashedPass, salt) = Utility.HashPassword(req.Password);

        long generatedUserId = await unitOfWork.Register.GenerateUserId();

        User user = new()
        {
            UserId = generatedUserId,
            UserName = req.UserName,
            Password = hashedPass,
            PhoneNumber = req.PhoneNumber,
            Salt = salt,
            EmailAddress = req.EmailAddress,
        };

        try
        {
            await unitOfWork.Register.AddAsync(user);
            await unitOfWork.SaveAsync();
            return 1;
        }
        catch
        {
            throw new Exception("Register failed");
        }
    }
}

