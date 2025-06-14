using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Abstractions;

/// <summary>
/// Interface for command/query validation following MySpot patterns
/// </summary>
public interface IValidator<in T>
{
    Result Validate(T instance);
}

/// <summary>
/// Base interface for objects that can be validated
/// </summary>
public interface IValidatable
{
    Result Validate();
}