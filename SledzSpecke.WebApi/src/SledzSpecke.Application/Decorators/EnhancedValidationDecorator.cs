using FluentValidation;
using FluentValidation.Results;
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

    public async Task<Core.Abstractions.Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        // Try to get FluentValidation validator first
        var validator = _serviceProvider.GetService<FluentValidation.IValidator<TCommand>>();
        
        if (validator != null)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                var errors = FormatValidationErrors(validationResult.Errors);
                var errorCode = GetPrimaryErrorCode(validationResult.Errors);
                return Core.Abstractions.Result.Failure(errors, errorCode);
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
                return Core.Abstractions.Result.Failure($"Validation failed: {string.Join("; ", errors)}", ErrorCodes.VALIDATION_ERROR);
            }
        }

        // If validation passes, execute the handler
        return await _handler.HandleAsync(command, cancellationToken);
    }

    private static string FormatValidationErrors(List<ValidationFailure> errors)
    {
        var groupedErrors = errors
            .GroupBy(e => e.PropertyName)
            .Select(g => $"{g.Key}: {string.Join(", ", g.Select(e => e.ErrorMessage))}")
            .ToList();

        return string.Join("; ", groupedErrors);
    }

    private static string GetPrimaryErrorCode(List<ValidationFailure> errors)
    {
        var specificError = errors.FirstOrDefault(e => 
            e.ErrorCode != null && 
            e.ErrorCode != ErrorCodes.VALIDATION_ERROR);
        
        return specificError?.ErrorCode ?? ErrorCodes.VALIDATION_ERROR;
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

    public async Task<Core.Abstractions.Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        // Try to get FluentValidation validator first
        var validator = _serviceProvider.GetService<FluentValidation.IValidator<TCommand>>();
        
        if (validator != null)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                var errors = FormatValidationErrors(validationResult.Errors);
                var errorCode = GetPrimaryErrorCode(validationResult.Errors);
                return Core.Abstractions.Result.Failure<TResult>(errors, errorCode);
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
                return Core.Abstractions.Result.Failure<TResult>($"Validation failed: {string.Join("; ", errors)}", ErrorCodes.VALIDATION_ERROR);
            }
        }

        // If validation passes, execute the handler
        return await _handler.HandleAsync(command, cancellationToken);
    }

    private static string FormatValidationErrors(List<ValidationFailure> errors)
    {
        var groupedErrors = errors
            .GroupBy(e => e.PropertyName)
            .Select(g => $"{g.Key}: {string.Join(", ", g.Select(e => e.ErrorMessage))}")
            .ToList();

        return string.Join("; ", groupedErrors);
    }

    private static string GetPrimaryErrorCode(List<ValidationFailure> errors)
    {
        var specificError = errors.FirstOrDefault(e => 
            e.ErrorCode != null && 
            e.ErrorCode != ErrorCodes.VALIDATION_ERROR);
        
        return specificError?.ErrorCode ?? ErrorCodes.VALIDATION_ERROR;
    }
}