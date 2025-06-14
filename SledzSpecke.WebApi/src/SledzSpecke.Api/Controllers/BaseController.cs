using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected async Task<ActionResult<T>> HandleAsync<T>(ICommand<T> command, ICommandHandler<ICommand<T>, T> handler)
    {
        var result = await handler.HandleAsync(command);
        return Ok(result);
    }

    protected async Task<ActionResult> HandleAsync<T>(T command, ICommandHandler<T> handler) where T : class, ICommand
    {
        await handler.HandleAsync(command);
        return Ok();
    }

    protected async Task<ActionResult<T>> HandleAsync<TQuery, T>(TQuery query, IQueryHandler<TQuery, T> handler)
        where TQuery : class, IQuery<T>
    {
        var result = await handler.HandleAsync(query);
        return Ok(result);
    }
}