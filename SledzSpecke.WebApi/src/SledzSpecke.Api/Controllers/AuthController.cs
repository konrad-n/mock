using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ICommandHandler<SignIn, JwtDto> _signInHandler;
    private readonly ICommandHandler<SignUp> _signUpHandler;

    public AuthController(ICommandHandler<SignIn, JwtDto> signInHandler, ICommandHandler<SignUp> signUpHandler)
    {
        _signInHandler = signInHandler;
        _signUpHandler = signUpHandler;
    }

    [HttpPost("sign-in")]
    public async Task<ActionResult<JwtDto>> SignIn(SignIn command)
        => Ok(await _signInHandler.HandleAsync(command));

    [HttpPost("sign-up")]
    public async Task<ActionResult> SignUp(SignUp command)
    {
        await _signUpHandler.HandleAsync(command);
        return Ok();
    }
}