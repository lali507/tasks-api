namespace TaskManager.Core.Results;

// GN-07: the title of every error response
public static class ErrorMessages
{
    public const string InvalidData = "הנתונים שנשלחו אינם תקינים";
    public const string InvalidParameter = "פרמטר לא תקין";
    public const string LoginRequired = "נדרשת התחברות";
    public const string InvalidCredentials = "אימייל או סיסמה שגויים";
    public const string Forbidden = "אין לך הרשאה לפעולה זו";
    public const string OnlyTakerCanComplete = "רק מי שלקח יכול לסמן";
    public const string AdminUserLocked = "לא ניתן לשנות משתמש מנהל";
    public const string NotFound = "לא נמצא";
    public const string EmailTaken = "האימייל כבר רשום";
    public const string ProjectNameTaken = "שם הפרויקט כבר קיים";
    public const string LabelNameTaken = "שם התווית כבר קיים";
    public const string TaskAlreadyTaken = "המשימה כבר נלקחה";
    public const string TaskNotTaken = "ניתן לסמן רק משימה שנלקחה";
    public const string TaskCompleted = "לא ניתן לעדכן משימה שהושלמה";
    public const string TaskChanged = "המשימה השתנתה, טען מחדש";
    public const string TaskTakenCannotDelete = "לא ניתן למחוק משימה שנלקחה";
    public const string ProjectHasTasks = "לא ניתן למחוק פרויקט שיש בו משימות";
    public const string UserHasActiveTask = "למשתמש יש משימה פעילה";
    public const string ServerError = "אירעה שגיאה";
}
