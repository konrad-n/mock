using SledzSpecke.Application.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace SledzSpecke.Application.Decorators;

/// <summary>
/// Decorator that adds validation to command handlers
/// Uses Data Annotations for simple validation until FluentValidation is integrated
/// </summary>
internal sealed class ValidationCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _handler;
    private readonly IServiceProvider _serviceProvider;

    public ValidationCommandHandlerDecorator(
        ICommandHandler<TCommand> handler,
        IServiceProvider serviceProvider)
    {
        _handler = handler;
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync(TCommand command)
    {
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new ValidationContext(command, _serviceProvider, null);

        if (!Validator.TryValidateObject(command, context, validationResults, validateAllProperties: true))
        {
            var errors = validationResults.Select(r => r.ErrorMessage ?? "Validation error").ToList();
            throw new SledzSpecke.Application.Exceptions.ValidationException($"Validation failed: {string.Join(", ", errors)}");
        }

        await _handler.HandleAsync(command);
    }
}

/// <summary>
/// Decorator for command handlers that return a result
/// </summary>
internal sealed class ValidationCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : class, ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _handler;
    private readonly IServiceProvider _serviceProvider;

    public ValidationCommandHandlerDecorator(
        ICommandHandler<TCommand, TResult> handler,
        IServiceProvider serviceProvider)
    {
        _handler = handler;
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> HandleAsync(TCommand command)
    {
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new ValidationContext(command, _serviceProvider, null);

        if (!Validator.TryValidateObject(command, context, validationResults, validateAllProperties: true))
        {
            var errors = validationResults.Select(r => r.ErrorMessage ?? "Validation error").ToList();
            throw new SledzSpecke.Application.Exceptions.ValidationException($"Validation failed: {string.Join(", ", errors)}");
        }

        return await _handler.HandleAsync(command);
    }
}