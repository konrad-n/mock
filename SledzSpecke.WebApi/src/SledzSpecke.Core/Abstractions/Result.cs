namespace SledzSpecke.Core.Abstractions;

public class Result
{
    protected Result(bool isSuccess, string error, string? errorCode = null)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Success result cannot have an error message");

        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Failed result must have an error message");

        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    public string? ErrorCode { get; }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error, string? errorCode = null) => new(false, error, errorCode);

    public static Result<T> Success<T>(T value) => new(value, true, string.Empty);
    public static Result<T> Failure<T>(string error, string? errorCode = null) => new(default, false, error, errorCode);
    
    public TResult Match<TResult>(
        Func<TResult> onSuccess,
        Func<string, string?, TResult> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error, ErrorCode);
}

public class Result<T> : Result
{
    private readonly T? _value;

    protected internal Result(T? value, bool isSuccess, string error, string? errorCode = null) : base(isSuccess, error, errorCode)
    {
        _value = value;
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result");

    public static new Result<T> Success(T value) => new(value, true, string.Empty);
    public static new Result<T> Failure(string error, string? errorCode = null) => new(default, false, error, errorCode);

    public static implicit operator Result<T>(T value) => Success(value);
    
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, string?, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value) : onFailure(Error, ErrorCode);
    
    public async Task<TResult> MatchAsync<TResult>(
        Func<T, Task<TResult>> onSuccess,
        Func<string, string?, Task<TResult>> onFailure) =>
        IsSuccess ? await onSuccess(Value) : await onFailure(Error, ErrorCode);
}