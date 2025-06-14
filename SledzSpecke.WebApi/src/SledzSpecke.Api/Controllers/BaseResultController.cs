using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseResultController : ControllerBase
{
    protected async Task<ActionResult<T>> HandleAsync<TCommand, T>(
        TCommand command, 
        IResultCommandHandler<TCommand, T> handler) 
        where TCommand : class, ICommand<T>
    {
        var result = await handler.HandleAsync(command);
        
        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }
        
        return Ok(result.Value);
    }

    protected async Task<ActionResult> HandleAsync<TCommand>(
        TCommand command, 
        IResultCommandHandler<TCommand> handler) 
        where TCommand : class, ICommand
    {
        var result = await handler.HandleAsync(command);
        
        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }
        
        return Ok();
    }

    protected async Task<ActionResult<T>> HandleAsync<TQuery, T>(
        TQuery query, 
        IQueryHandler<TQuery, T> handler)
        where TQuery : class, IQuery<T>
    {
        var result = await handler.HandleAsync(query);
        return Ok(result);
    }
    
    protected ActionResult<T> HandleResult<T>(Result<T> result)
    {
        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }
        
        return Ok(result.Value);
    }
    
    protected ActionResult HandleResult(Result result)
    {
        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }
        
        return Ok();
    }
}