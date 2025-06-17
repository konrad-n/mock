using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Core.Extensions;

public static class ResultExtensions
{
    /// <summary>
    /// Maps a successful result value to a new type
    /// </summary>
    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(
        this Task<Result<TIn>> resultTask, 
        Func<TIn, Task<TOut>> mapper)
    {
        var result = await resultTask;
        
        if (!result.IsSuccess)
            return Result<TOut>.Failure(result.Error!, result.ErrorCode!);
            
        var mappedValue = await mapper(result.Value);
        return Result<TOut>.Success(mappedValue);
    }
    
    /// <summary>
    /// Maps a successful result value synchronously
    /// </summary>
    public static Result<TOut> Map<TIn, TOut>(
        this Result<TIn> result, 
        Func<TIn, TOut> mapper)
    {
        if (!result.IsSuccess)
            return Result<TOut>.Failure(result.Error!, result.ErrorCode!);
            
        var mappedValue = mapper(result.Value);
        return Result<TOut>.Success(mappedValue);
    }
    
    /// <summary>
    /// Executes an action if the result is successful
    /// </summary>
    public static async Task<Result> ThenAsync<T>(
        this Task<Result<T>> resultTask,
        Func<T, Task> action)
    {
        var result = await resultTask;
        
        if (!result.IsSuccess)
            return Result.Failure(result.Error!, result.ErrorCode!);
            
        await action(result.Value);
        return Result.Success();
    }
    
    /// <summary>
    /// Chains multiple result-returning operations
    /// </summary>
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
        this Task<Result<TIn>> resultTask,
        Func<TIn, Task<Result<TOut>>> binder)
    {
        var result = await resultTask;
        
        if (!result.IsSuccess)
            return Result<TOut>.Failure(result.Error!, result.ErrorCode!);
            
        return await binder(result.Value);
    }
    
    /// <summary>
    /// Provides a default value if the result is a failure
    /// </summary>
    public static async Task<T> GetValueOrDefaultAsync<T>(
        this Task<Result<T>> resultTask,
        T defaultValue)
    {
        var result = await resultTask;
        return result.IsSuccess ? result.Value : defaultValue;
    }
}