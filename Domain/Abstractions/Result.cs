namespace Domain.Abstractions;

public enum ErrorType
{
    Validation,
    NotFound,
    Unauthorized,
    Forbidden,
    Conflict,
    Failure
}

public sealed record Error(string Code, string Message, ErrorType ErrorType)
{
    public static Error Validation(string code, string message)
        => new(code, message, ErrorType.Validation);

    public static Error Conflict(string code, string message)
        => new(code, message, ErrorType.Conflict);

    public static Error NotFound(string code, string message)
        => new(code, message, ErrorType.NotFound);

    public static Error Unauthorized(string code, string message)
        => new(code, message, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string message)
        => new(code, message, ErrorType.Forbidden);

    public static Error Failure(string code, string message)
        => new(code, message, ErrorType.Failure);
}

public class Result
{
    public bool IsSuccess { get; }

    public Error? Error { get; }

    protected Result(
        bool isSuccess,
        Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success()
        => new(true, null);

    public static Result Failure(Error error)
        => new(false, error);


}


public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(
        bool isSuccess,
        Error? error,
        T? value
        ) : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value)
        => new(true, null, value);

    public static new Result<T> Failure(Error error)
        => new(false, error, default);
}