using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseResultController
{
    private readonly IResultCommandHandler<SignIn, JwtDto> _signInHandler;
    private readonly IResultCommandHandler<SignUp> _signUpHandler;

    public AuthController(
        IResultCommandHandler<SignIn, JwtDto> signInHandler, 
        IResultCommandHandler<SignUp> signUpHandler)
    {
        _signInHandler = signInHandler;
        _signUpHandler = signUpHandler;
    }

    [HttpPost("sign-in")]
    public async Task<ActionResult<JwtDto>> SignIn(SignIn command)
    {
        var result = await _signInHandler.HandleAsync(command);
        return HandleResult(result);
    }

    [HttpPost("sign-up")]
    public async Task<ActionResult> SignUp(SignUp command)
    {
        var result = await _signUpHandler.HandleAsync(command);
        return HandleResult(result);
    }
}