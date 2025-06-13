using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;

namespace SledzSpecke.Api.Controllers;

[Authorize]
public class UsersController : BaseController
{
    private readonly IQueryHandler<GetUser, UserDto> _getUserHandler;
    private readonly IQueryHandler<GetUsers, IEnumerable<UserDto>> _getUsersHandler;

    public UsersController(IQueryHandler<GetUser, UserDto> getUserHandler,
        IQueryHandler<GetUsers, IEnumerable<UserDto>> getUsersHandler)
    {
        _getUserHandler = getUserHandler;
        _getUsersHandler = getUsersHandler;
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<UserDto>> Get(int userId)
        => await HandleAsync(new GetUser(userId), _getUserHandler);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> Get()
        => await HandleAsync(new GetUsers(), _getUsersHandler);
}