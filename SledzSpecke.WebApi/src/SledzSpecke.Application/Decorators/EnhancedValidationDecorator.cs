using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace SledzSpecke.Application.Decorators;

/// <summary>
/// Enhanced validation decorator that works with Result pattern and custom validators
/// </summary>
public class EnhancedValidationDecorator<TCommand> : IResultCommandHandler<TCommand> 
    where TCommand : class, ICommand
{
    private readonly IResultCommandHandler<TCommand> _handler;
    private readonly IServiceProvider _serviceProvider;

    public EnhancedValidationDecorator(
        IResultCommandHandler<TCommand> handler,
        IServiceProvider serviceProvider)
    {
        _handler = handler;
        _serviceProvider = serviceProvider;
    }

    public async Task<Result> HandleAsync(TCommand command)
    {
        // Try to get custom validator first
        var validator = _serviceProvider.GetService<IValidator<TCommand>>();
        
        if (validator != null)
        {
            var validationResult = validator.Validate(command);
            if (validationResult.IsFailure)
            {
                return validationResult;
            }
        }
        else
        {
            // Fall back to data annotations validation
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(command, _serviceProvider, null);

            if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
                command, context, validationResults, validateAllProperties: true))
            {
                var errors = validationResults.Select(r => r.ErrorMessage ?? "Validation error");
                return Result.Failure($"Validation failed: {string.Join("; ", errors)}");
            }
        }

        // If validation passes, execute the handler
        return await _handler.HandleAsync(command);
    }
}

/// <summary>
/// Enhanced validation decorator for commands that return a result
/// </summary>
public class EnhancedValidationDecorator<TCommand, TResult> : IResultCommandHandler<TCommand, TResult>
    where TCommand : class, ICommand<TResult>
{
    private readonly IResultCommandHandler<TCommand, TResult> _handler;
    private readonly IServiceProvider _serviceProvider;

    public EnhancedValidationDecorator(
        IResultCommandHandler<TCommand, TResult> handler,
        IServiceProvider serviceProvider)
    {
        _handler = handler;
        _serviceProvider = serviceProvider;
    }

    public async Task<Result<TResult>> HandleAsync(TCommand command)
    {
        // Try to get custom validator first
        var validator = _serviceProvider.GetService<IValidator<TCommand>>();
        
        if (validator != null)
        {
            var validationResult = validator.Validate(command);
            if (validationResult.IsFailure)
            {
                return Result.Failure<TResult>(validationResult.Error);
            }
        }
        else
        {
            // Fall back to data annotations validation
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(command, _serviceProvider, null);

            if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
                command, context, validationResults, validateAllProperties: true))
            {
                var errors = validationResults.Select(r => r.ErrorMessage ?? "Validation error");
                return Result.Failure<TResult>($"Validation failed: {string.Join("; ", errors)}");
            }
        }

        // If validation passes, execute the handler
        return await _handler.HandleAsync(command);
    }
}