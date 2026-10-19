# מסמך ניתוח 1 – Core: תפקידים, ישויות ו-DTOs

מערכת ניהול משימות .NET. שכבה: Core. משמש את משימות הפיתוח 1–6 (ראו dev-tasks.md). מזהים ממסמכים אחרים – לפי האינדקס (00-index.md).

## תקציר
- מערכת לניהול משימות בצוות: .NET Web API, PostgreSQL, לקוח React.
- ישויות: User, Project, TaskItem, Label.
- תפקידים: Admin (ניהול פרויקטים, משימות, תוויות ומשתמשים מסוג Member), Member (צפייה, לקיחת משימה, סימון כהושלמה).
- משאב מוגבל: משימה במצב Open – נלקחת ע"י משתמש אחד בלבד.
- טיפול בתחרות: Optimistic concurrency עם xmin; בלקיחה מקבילה – הצלחה אחת ו-409 אחד.
- מחוץ לתחום: ענן, Docker, אחסון קבצים, Google login, caching, SignalR, CI.

## תפקידים והרשאות
משתמשים: אורח לא מזוהה, Member, Admin.

- כלל RL-01: הרשמה עצמית יוצרת תמיד Member. Admin נוצר רק מ-Seed. Admin יוצר משתמשים מסוג Member בלבד. (עמימות 1)
- כלל RL-02: Member רואה את כל הפרויקטים, המשימות והתוויות – קריאה בלבד. (עמימות 2)
- כלל RL-03: רק Member לוקח משימה, ורק לעצמו. Admin אינו לוקח ואינו משייך משימות. (עמימות 3)
- כלל RL-04: זיהוי Admin – התחברות באימייל וסיסמה כמו כל משתמש; הטוקן נושא claim של Role = Admin, ופעולות מנהל מוגנות ב-Authorize לפי Role. אין אימות נוסף למנהל; יצוין ב-README כמגבלה ידועה.

| פעולה | אורח | Member | Admin |
|---|---|---|---|
| הרשמה / התחברות | ✓ | ✓ | ✓ |
| צפייה בפרויקטים, משימות, תוויות | ✗ | ✓ | ✓ |
| יצירה/עדכון/מחיקה – Project | ✗ | ✗ | ✓ |
| יצירה/עדכון/מחיקה – TaskItem | ✗ | ✗ | ✓ |
| יצירה/עדכון/מחיקה – Label | ✗ | ✗ | ✓ |
| ניהול משתמשים – צפייה/הוספה/עריכה/מחיקה (Members בלבד) | ✗ | ✗ | ✓ |
| לקיחת משימה | ✗ | ✓ | ✗ |
| סימון כהושלמה (רק מי שלקח) | ✗ | ✓ | ✗ |

### ניהול משתמשים – Admin בלבד

| פעולה | כתובת | קלט | תוצאה |
|---|---|---|---|
| רשימה | GET /api/users | page, pageSize | 200 – Id, Email, DisplayName, Role, CreatedAt |
| הוספה | POST /api/users | Email, DisplayName, Password | 201 + Location; נוצר Member תמיד |
| עריכה | PUT /api/users/{id} | Email, DisplayName, NewPassword (רשות) | 200 עם המשתמש |
| מחיקה | DELETE /api/users/{id} | – | 204 |

- כלל UM-01: ולידציות כמו בהרשמה (EN-01, EN-01-R1); Email כפול ← 409.
- כלל UM-02: Role אינו נשלח ואינו ניתן לשינוי.
- כלל UM-03: עריכה או מחיקה של Admin (כולל עצמו) ← 403.
- כלל UM-04: מחיקה מותרת למשתמש בלי משימות או שכל משימותיו Done; משימה Taken ← 409 "למשתמש יש משימה פעילה".
- כלל UM-05: במחיקה – במשימות Done של המשתמש TakenByUserId ← null; המשימה נשארת Done עם CompletedAt.
- כלל UM-06: משתמש לא קיים ← 404.
- כלל UM-07: טוקן של משתמש שנמחק – כל פעולת כתיבה ← 401.

## מודל נתונים
טבלאות השדות מתארות עמודות בבסיס הנתונים בלבד. Navigation properties (שדות קשר בקוד שאינם עמודות) מתועדים בהערות. מפתח זר (FK) נשמר תמיד בצד ה"רבים" של הקשר; קשר many-to-many נשמר בטבלת קישור.

### EN-01: User

| שדה | סוג | חובה | כללים | ברירת מחדל | מי מעדכן |
|---|---|---|---|---|---|
| Id | int | כן | מופק אוטומטית | – | מערכת |
| Email | string | כן | עד 256 תווים; פורמט אימייל תקין; ייחודי (אינדקס ייחודי); לא רגיש לאותיות – נשמר אחרי Trim ובאותיות קטנות | – | המשתמש בהרשמה / Admin |
| DisplayName | string | כן | 2–50 תווים; אחרי Trim | – | המשתמש בהרשמה / Admin |
| PasswordHash | string | כן | hash בלבד; לעולם לא מוחזר בתשובה | – | מערכת |
| Role | enum (Admin / Member) | כן | הרשמה והוספה ע"י Admin יוצרות Member בלבד; אינו ניתן לשינוי | Member | Seed בלבד ל-Admin |
| CreatedAt | DateTime (UTC) | כן | נקבע בשרת | זמן היצירה | מערכת |

- כלל EN-01-R1: סיסמה (קלט בהרשמה / הוספה / עריכה): 8–64 תווים, לפחות אות אחת וספרה אחת. (עמימות 5)
- כלל EN-01-R2: התחברות לפי Email + סיסמה. (עמימות 4)
- כלל EN-01-R3: המשתמש אינו עורך את עצמו; Admin עורך Members בלבד לפי UM-01 עד UM-03.
- כלל EN-01-R4: אין שליחת אימייל ואין שחזור סיסמה.
- כלל EN-01-R5: תשובת התחברות/הרשמה מחזירה DisplayName.
- כלל EN-01-R6: TakenTasks (ICollection<TaskItem>) – navigation בלבד; הקשר נשמר ב-TaskItem.TakenByUserId.
- כלל EN-01-R7: ניהול משתמשים – ראו UM-01 עד UM-07 (פרק 2).

### EN-02: Project

| שדה | סוג | חובה | כללים | ברירת מחדל | מי מעדכן |
|---|---|---|---|---|---|
| Id | int | כן | מופק אוטומטית | – | מערכת |
| Name | string | כן | 2–100 תווים; אחרי Trim; ייחודי לא רגיש לאותיות (אינדקס ייחודי) | – | Admin |
| Description | string? | לא | עד 1000 תווים | null | Admin |
| CreatedAt | DateTime (UTC) | כן | נקבע בשרת | זמן היצירה | מערכת |

- כלל EN-02-R1: שם כפול ביצירה או בעדכון ← 409. (עמימות 6)
- כלל EN-02-R2: Tasks (ICollection<TaskItem>) – navigation בלבד. הקשר נשמר ב-TaskItem.ProjectId.
- כלל EN-02-R3: DTOs לתשובה:
  - תשובת ProjectResponse – Id, Name, Description, CreatedAt, TaskCount. מוחזר ב-GET /api/projects. TaskCount מחושב בשאילתה (COUNT), ללא Include.
  - תשובת ProjectDetailsResponse – כל שדות ProjectResponse + רשימת Tasks מלאה (TaskResponse כולל תוויות). מוחזר ב-GET /api/projects/{id} בלבד; נטען עם Include ו-ThenInclude; 404 אם לא קיים.
- כלל EN-02-R4: TaskCount הוא שדה DTO בלבד – אינו נשמר בבסיס הנתונים.

### EN-03: Label

| שדה | סוג | חובה | כללים | ברירת מחדל | מי מעדכן |
|---|---|---|---|---|---|
| Id | int | כן | מופק אוטומטית | – | מערכת |
| Name | string | כן | 2–30 תווים; אחרי Trim; ייחודי לא רגיש לאותיות (אינדקס ייחודי) | – | Admin |

- כלל EN-03-R1: שם כפול ביצירה או בעדכון ← 409. (עמימות 7)
- כלל EN-03-R2: Tasks (ICollection<TaskItem>) – navigation בלבד, צד אחד של קשר many-to-many. יחד עם TaskItem.Labels, EF Core יוצר טבלת קישור TaskItemLabel (TaskItemId, LabelId).
- כלל EN-03-R3: אין צבע ואין תיאור לתווית.

### EN-04: TaskItem

| שדה | סוג | חובה | כללים | ברירת מחדל | מי מעדכן |
|---|---|---|---|---|---|
| Id | int | כן | מופק אוטומטית | – | מערכת |
| Title | string | כן | 2–100 תווים; אחרי Trim | – | Admin |
| Description | string? | לא | עד 1000 תווים | null | Admin |
| Status | enum (Open / Taken / Done) | כן | מעברים לפי ST-01 (מסמך 3) | Open | מערכת (בלקיחה/השלמה) |
| ProjectId | int (FK) | כן | פרויקט קיים | – | Admin |
| TakenByUserId | int? (FK) | לא | ריק כש-Status=Open; ממולא כש-Taken; כש-Done – ממולא, או null אם המשתמש נמחק (UM-05) | null | מערכת (בלקיחה / במחיקת משתמש) |
| CreatedAt | DateTime (UTC) | כן | נקבע בשרת | זמן היצירה | מערכת |
| CompletedAt | DateTime? (UTC) | לא | נקבע בשרת במעבר ל-Done; ריק בשאר המצבים | null | מערכת (בהשלמה) |
| xmin | uint | כן | Concurrency token – עמודת מערכת של PostgreSQL | – | PostgreSQL |

- כלל EN-04-R1: Navigation בלבד (לא עמודות): Project, TakenByUser, Labels (ICollection<Label>).
- כלל EN-04-R2: תוויות אינן עמודה – נשמרות בטבלת הקישור TaskItemLabel. ב-API: קלט LabelIds, פלט רשימת תוויות.
- כלל EN-04-R3: xmin קיים בכל שורה ב-PostgreSQL; ההגדרה ב-Fluent API ממפה אותו כ-Concurrency token בלבד, ואינה מוסיפה עמודה. (עמימות 10)
- כלל EN-04-R4: אין תאריך יעד (DueDate).
- כלל EN-04-R5: Migrations (עמימות 9):
  - מיגרציה 1 – כל הטבלאות והשדות, כולל מיפוי xmin, למעט CompletedAt.
  - מיגרציה 2 – הוספת CompletedAt ל-TaskItem.

## DTOs

### DT-00: רשימת DTOs

| משאב | DTO | מטרה | שדות |
|---|---|---|---|
| Auth | RegisterRequest | קלט להרשמה עצמית של אורח | Email, DisplayName, Password |
| Auth | LoginRequest | קלט להתחברות | Email, Password |
| Auth | AuthResponse | תשובה להרשמה והתחברות – טוקן ופרטי המשתמש המחובר | Token, DisplayName, Role |
| Users | CreateUserRequest | קלט להוספת Member ע"י Admin | Email, DisplayName, Password |
| Users | UpdateUserRequest | קלט לעריכת Member ע"י Admin | Email, DisplayName, NewPassword? |
| Users | UserResponse | הצגת משתמש – כפריט ברשימה, או כמשתמש היחיד שהוחזר בהוספה/עריכה | Id, Email, DisplayName, Role, CreatedAt |
| Projects | ProjectRequest | קלט ליצירה ולעדכון פרויקט | Name, Description |
| Projects | ProjectResponse | הצגת פרויקט – כפריט ברשימה, או כפרויקט היחיד שהוחזר ביצירה/עדכון | Id, Name, Description, CreatedAt, TaskCount |
| Projects | ProjectDetailsResponse | הצגת פרויקט אחד עם כל משימותיו | ProjectResponse + Tasks (רשימת TaskResponse) |
| Labels | LabelRequest | קלט ליצירה ולעדכון תווית | Name |
| Labels | LabelResponse | הצגת תווית – כפריט ברשימה, כתווית היחידה שהוחזרה ביצירה/עדכון, או בתוך משימה | Id, Name |
| Tasks | TaskRequest | קלט ליצירה ולעדכון משימה ע"י Admin, כולל שיוך תוויות | Title, Description, ProjectId, LabelIds |
| Tasks | TaskResponse | הצגת משימה – ברשימה, בפרטים ובתוך פרויקט | ראו DT-13 |
| כללי | PagedResponse<T> | עטיפה לכל רשימה עם חלוקה לעמודים | Items, TotalCount, Page, PageSize |

- כלל DT-00-R1: RegisterRequest ו-CreateUserRequest נפרדים – כל endpoint עם חוזה משלו; הרשמה מחזירה AuthResponse, הוספה ע"י Admin מחזירה UserResponse בלי טוקן.
- כלל DT-00-R2: תשובה ליצירה/עדכון מחזירה רק את המשאב שנוצר או עודכן; רשימה מעודכנת – בבקשת GET נפרדת.
- כלל DT-00-R3: PagedResponse<T> – Items הוא תוכן העמוד המבוקש; TotalCount הוא סך כל הפריטים (לחישוב מספר העמודים בלקוח); הספירה והחלוקה לעמודים נעשות בתוך השאילתה.
- כלל DT-00-R4: מחוץ לפרק זה – פרמטרי סינון ו-pagination ומבנה השגיאה (GN-02, GN-03 – מסמך 4); גוף בקשת לקיחה והשלמה (FN-MB-06/07).
- כלל DT-00-R5: Enums (Role, Status) מוחזרים כמחרוזת ("Admin", "Open") ולא כמספר.

### DTOs של קלט

| מזהה | DTO | שדה | סוג | חובה | Data Annotations |
|---|---|---|---|---|---|
| DT-01 | RegisterRequest | Email | string | כן | Required, EmailAddress, MaxLength 256 |
| | | DisplayName | string | כן | Required, StringLength 2–50 |
| | | Password | string | כן | Required, StringLength 8–64, לפחות אות אחת וספרה אחת |
| DT-02 | LoginRequest | Email | string | כן | Required, EmailAddress, MaxLength 256 |
| | | Password | string | כן | Required בלבד |
| DT-04 | CreateUserRequest | Email, DisplayName, Password | | | כמו DT-01 |
| DT-05 | UpdateUserRequest | Email | string | כן | כמו DT-01 |
| | | DisplayName | string | כן | כמו DT-01 |
| | | NewPassword | string? | לא | אם נשלח – כמו Password ב-DT-01 |
| DT-07 | ProjectRequest | Name | string | כן | Required, StringLength 2–100 |
| | | Description | string? | לא | MaxLength 1000 |
| DT-10 | LabelRequest | Name | string | כן | Required, StringLength 2–30 |
| DT-12 | TaskRequest | Title | string | כן | Required, StringLength 2–100 |
| | | Description | string? | לא | MaxLength 1000 |
| | | ProjectId | int | כן | Required, Range מ-1 |
| | | LabelIds | List<int>? | לא | – |

- כלל DT-01-R2: אין שדה Role ב-RegisterRequest וב-CreateUserRequest – התפקיד נקבע בשרת תמיד Member (RL-01).
- כלל DT-02-R1: ב-LoginRequest אין בדיקת אורך ומורכבות לסיסמה – סיסמה שגויה מטופלת כ-401 (FN-GU-02).
- כלל DT-12-R3: אין Status ואין TakenByUserId ב-TaskRequest.

### DTOs של פלט

| מזהה | DTO | שדות (סוג) |
|---|---|---|
| DT-03 | AuthResponse | Token (string), DisplayName (string), Role (string) |
| DT-06 | UserResponse | Id (int), Email (string), DisplayName (string), Role (string), CreatedAt (DateTime UTC) – לעולם בלי PasswordHash |
| DT-08 | ProjectResponse | Id (int), Name (string), Description (string?), CreatedAt (DateTime UTC), TaskCount (int) |
| DT-09 | ProjectDetailsResponse | כל שדות DT-08 + Tasks (List<TaskResponse>) |
| DT-11 | LabelResponse | Id (int), Name (string) |
| DT-13 | TaskResponse | Id (int), Title (string), Description (string?), Status (string), ProjectId (int), ProjectName (string), TakenByUserId (int?), TakenByDisplayName (string?), CreatedAt (DateTime UTC), CompletedAt (DateTime? UTC), Labels (List<LabelResponse>) |
| DT-14 | PagedResponse<T> | Items (List<T>), TotalCount (int), Page (int), PageSize (int) |

- כלל DT-13-R1: xmin אינו מוחזר בתשובה.

## Enums
- כלל CO-01: Role – Admin / Member (EN-01). Status – Open / Taken / Done; המעברים לפי ST-01 (מסמך 3).
- כלל CO-02: Enums מוחזרים בתשובות כמחרוזת (DT-00-R5).

## חוזים ב-Core
- כלל CO-03: Core אינו מפנה לשום פרויקט ולשום חבילה חיצונית.
- כלל CO-04: Interface של Repository לכל ישות, ו-Interface של Service לכל תחום: Auth, Users, Projects, Tasks, Labels. כל הפעולות אסינכרוניות ומקבלות CancellationToken.
- כלל CO-05: ה-Service מחזיר אובייקט תוצאה ולא קוד HTTP. סוגי התוצאה: הצלחה, ולידציה (400), לא מזוהה (401), אין הרשאה (403), לא נמצא (404), התנגשות (409); כל תוצאה שאינה הצלחה נושאת את הודעת ה-title (GN-07, מסמך 4).
