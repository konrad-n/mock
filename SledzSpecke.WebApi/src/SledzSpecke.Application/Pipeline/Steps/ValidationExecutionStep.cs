using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Pipeline;
using System.ComponentModel.DataAnnotations;

namespace SledzSpecke.Application.Pipeline.Steps;

public sealed class ValidationExecutionStep : IMessageExecutionStep
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationExecutionStep(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task ExecuteAsync<TMessage>(TMessage message, Func<Task> next, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        // For now, use DataAnnotations validation until FluentValidation is properly configured
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(message, _serviceProvider, null);

        if (!Validator.TryValidateObject(message, context, validationResults, validateAllProperties: true))
        {
            var errors = validationResults.Select(r => r.ErrorMessage ?? "Validation error").ToList();
            throw new SledzSpecke.Application.Exceptions.ValidationException($"Validation failed: {string.Join(", ", errors)}");
        }

        return next();
    }
}