namespace TodoApp.Application.Common;

public sealed record OperationResult<T>(bool Success, T? Value, string? Error, string? ErrorCode)
{
    public static OperationResult<T> Ok(T value) => new(true, value, null, null);
    public static OperationResult<T> Fail(string error, string errorCode) => new(false, default, error, errorCode);
}
