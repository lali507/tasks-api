namespace TaskManager.Core.Results;

// CO-05: services return this instead of HTTP codes; every failure carries its GN-07 title
public class ServiceResult
{
    public ResultStatus Status { get; }
    public string? Title { get; }

    // Only for Validation: field name -> messages (GN-03 errors)
    public IReadOnlyDictionary<string, string[]>? Errors { get; }

    public bool IsSuccess => Status == ResultStatus.Success;

    protected ServiceResult(ResultStatus status, string? title = null, IReadOnlyDictionary<string, string[]>? errors = null)
    {
        Status = status;
        Title = title;
        Errors = errors;
    }

    public static ServiceResult Success() => new(ResultStatus.Success);
    public static ServiceResult Validation(IReadOnlyDictionary<string, string[]> errors) => new(ResultStatus.Validation, ErrorMessages.InvalidData, errors);
    public static ServiceResult Unauthorized(string title = ErrorMessages.LoginRequired) => new(ResultStatus.Unauthorized, title);
    public static ServiceResult Forbidden(string title = ErrorMessages.Forbidden) => new(ResultStatus.Forbidden, title);
    public static ServiceResult NotFound() => new(ResultStatus.NotFound, ErrorMessages.NotFound);
    public static ServiceResult Conflict(string title) => new(ResultStatus.Conflict, title);
}

public class ServiceResult<T> : ServiceResult
{
    public T? Value { get; }

    private ServiceResult(T value) : base(ResultStatus.Success) => Value = value;

    private ServiceResult(ResultStatus status, string title, IReadOnlyDictionary<string, string[]>? errors = null)
        : base(status, title, errors) { }

    public static ServiceResult<T> Success(T value) => new(value);
    public static new ServiceResult<T> Validation(IReadOnlyDictionary<string, string[]> errors) => new(ResultStatus.Validation, ErrorMessages.InvalidData, errors);
    public static new ServiceResult<T> Unauthorized(string title = ErrorMessages.LoginRequired) => new(ResultStatus.Unauthorized, title);
    public static new ServiceResult<T> Forbidden(string title = ErrorMessages.Forbidden) => new(ResultStatus.Forbidden, title);
    public static new ServiceResult<T> NotFound() => new(ResultStatus.NotFound, ErrorMessages.NotFound);
    public static new ServiceResult<T> Conflict(string title) => new(ResultStatus.Conflict, title);
}
