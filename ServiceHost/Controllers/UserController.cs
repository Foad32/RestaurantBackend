using Microsoft.AspNetCore.Mvc;
using Restaurant.Core.ApplicationService.Features.RegisterUser.Commands.Request;
using Restaurant.Core.Domain.DTOs;
using MediatR;

namespace ServiceHost.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class UserController(IMediator mediator) : Controller
    {
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser(RegisterUserDTO registerUserDTO)
        {
            try
            {
                var result = await mediator.Send(new RegisterUserCommand { RegisterUserDTO = registerUserDTO });
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal sever error");
            }
        }
    }
}
