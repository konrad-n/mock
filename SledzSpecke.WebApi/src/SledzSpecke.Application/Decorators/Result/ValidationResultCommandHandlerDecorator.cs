using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Decorators.Result;

internal sealed class ValidationResultCommandHandlerDecorator<TCommand, TResult> 
    : IResultCommandHandler<TCommand, TResult>
    where TCommand : class, ICommand<TResult>
{
    private readonly IResultCommandHandler<TCommand, TResult> _handler;
    private readonly IServiceProvider _serviceProvider;

    public ValidationResultCommandHandlerDecorator(
        IResultCommandHandler<TCommand, TResult> handler,
        IServiceProvider serviceProvider)
    {
        _handler = handler;
        _serviceProvider = serviceProvider;
    }

    public async Task<Result<TResult>> HandleAsync(
        TCommand command, 
        CancellationToken cancellationToken = default)
    {
        // Try to find a FluentValidation validator
        var validator = _serviceProvider.GetService<IValidator<TCommand>>();
        
        if (validator is not null)
        {
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());
                        
                return Result<TResult>.ValidationFailure(errors);
            }
        }
        
        return await _handler.HandleAsync(command, cancellationToken);
    }
}

internal sealed class ValidationResultCommandHandlerDecorator<TCommand> 
    : IResultCommandHandler<TCommand>
    where TCommand : class, ICommand
{
    private readonly IResultCommandHandler<TCommand> _handler;
    private readonly IServiceProvider _serviceProvider;

    public ValidationResultCommandHandlerDecorator(
        IResultCommandHandler<TCommand> handler,
        IServiceProvider serviceProvider)
    {
        _handler = handler;
        _serviceProvider = serviceProvider;
    }

    public async Task<Result> HandleAsync(
        TCommand command, 
        CancellationToken cancellationToken = default)
    {
        // Try to find a FluentValidation validator
        var validator = _serviceProvider.GetService<IValidator<TCommand>>();
        
        if (validator is not null)
        {
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());
                
                // Convert to Result (non-generic) with validation error
                return Result.Failure(
                    $"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}", 
                    "VALIDATION_ERROR");
            }
        }
        
        return await _handler.HandleAsync(command, cancellationToken);
    }
}