namespace TaskManager.Core.Results;

// CO-05: the API turns each status into its HTTP code
public enum ResultStatus
{
    Success,
    Validation,   // 400
    Unauthorized, // 401
    Forbidden,    // 403
    NotFound,     // 404
    Conflict      // 409
}
