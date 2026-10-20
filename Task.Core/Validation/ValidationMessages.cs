namespace TaskManager.Core.Validation;

// GN-07-R1: field messages inside errors. {1}/{2} are filled in by the attribute (max/min).
public static class ValidationMessages
{
    public const string Required = "שדה חובה";
    public const string Length = "אורך בין {2} ל-{1} תווים";
    public const string MaxLength = "אורך עד {1} תווים";
    public const string Email = "פורמט אימייל לא תקין";
    public const string PasswordComplexity = "הסיסמה חייבת לכלול אות וספרה";

    // FN-AD-09-R1: a project or label id in the body that does not exist (errors.ProjectId / errors.LabelIds)
    public const string NotFound = "לא נמצא";

    // EN-01-R1: at least one letter (any language) and one digit
    public const string PasswordPattern = @"^(?=.*\p{L})(?=.*[0-9]).*$";
}
