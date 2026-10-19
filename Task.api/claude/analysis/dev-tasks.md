# משימות פיתוח – מערכת ניהול משימות .NET

49 משימות לפי סדר השכבות: Core ← Data ← Service ← API ← תשתית ואבטחה ← בדיקות ← לקוח ← פריסה ← הגשה. כל משימה מפנה למסמך ניתוח אחד (ראו 00-index.md), ונסגרת רק כשקריטריון הסיום מתקיים. משימות הפריסה 46–49 מבוצעות לפני משימה 45 (README והגשה).

## מסמך 1 – Core (משימות 1–6)

| # | משימה | פירוט | הפניות | סיום |
|---|---|---|---|---|
| 1 | הכנה | שני repositories (שרת ולקוח), gitignore ל-bin, obj, node_modules, ‎.env וסודות; Docker Desktop; PostgreSQL בקונטיינר עם בסיס לפיתוח ובסיס לבדיקות (docker-compose); User Secrets ל-Connection String ולמפתח JWT | IN-01, IN-03, DP-02 (מסמך 5) | שני repositories עם commit ראשון; הקונטיינר עולה ושני הבסיסים קיימים |
| 2 | Solution | חמישה פרויקטים: Core, Data, Service, API, Tests; הפניות לפי כיוון התלות | CO-03 | ה-Solution מתקמפל; ב-Core אין הפניה חיצונית |
| 3 | ישויות ו-Enums | User, Project, TaskItem, Label עם שדות ה-navigation; Role ו-Status | EN-01 עד EN-04, CO-01 | הישויות תואמות את טבלאות השדות |
| 4 | DTOs של קלט | RegisterRequest, LoginRequest, CreateUserRequest, UpdateUserRequest, ProjectRequest, LabelRequest, TaskRequest עם Data Annotations | DT-00, DT-01 עד DT-12 | כל השדות והאנוטציות לפי הטבלה |
| 5 | DTOs של פלט | AuthResponse, UserResponse, ProjectResponse, ProjectDetailsResponse, LabelResponse, TaskResponse, PagedResponse | DT-03 עד DT-14, DT-13-R1 | אין PasswordHash ואין xmin בשום DTO |
| 6 | חוזים | Interface של Repository לכל ישות, Interface של Service לכל תחום, אובייקט תוצאה | CO-04, CO-05 | כל החוזים async עם CancellationToken |

## מסמך 2 – Data (משימות 7–13)

| # | משימה | פירוט | הפניות | סיום |
|---|---|---|---|---|
| 7 | DbContext | חבילות Npgsql ו-EF Core; DbContext עם DbSet לכל ישות | ERD | ה-DbContext מתקמפל |
| 8 | Fluent API | קשרים RE-01 עד RE-03 עם התנהגות מחיקה; אינדקסים ייחודיים; מיפוי xmin ב-TaskItem בלבד | RE-01 עד RE-03, DA-02, GN-11, EN-04-R3 | המיפוי תואם את ה-ERD |
| 9 | Repositories: Users, Projects, Labels | CRUD, pagination בשאילתה, AsNoTracking, ספירת TaskCount בשאילתה | DA-03, EN-02-R3 | כל פעולות החוזה ממומשות |
| 10 | Repository: Tasks | סינון status, projectId, mine; מיון; pagination; Include לתוויות, פרויקט ומשתמש | DA-03, FN-MB-03 (מסמך 3) | שאילתה אחת לרשימה, בלי שאילתה בלולאה |
| 11 | מיגרציה 1 | כל הטבלאות והשדות כולל xmin, בלי CompletedAt | DA-01 | הבסיס נוצר בפקודה אחת |
| 12 | מיגרציה 2 | הוספת CompletedAt | DA-01 | שתי מיגרציות קיימות ומוחלות |
| 13 | Seed | 30 משתמשים, 10 פרויקטים, 10 תוויות, 100 משימות Open | GN-05 | עדכון הבסיס טוען את הנתונים בלי התערבות ידנית |

## מסמך 3 – Service (משימות 14–22)

| # | משימה | פירוט | הפניות | סיום |
|---|---|---|---|---|
| 14 | AutoMapper | Profiles מישויות ל-DTOs של פלט | SV-01 | כל המיפויים מוגדרים |
| 15 | ProjectService | רשימה, פרטים, יצירה, עדכון, מחיקה; Trim; שם ייחודי | FN-MB-01, FN-MB-02, FN-AD-06 עד FN-AD-08, RE-01-R1 | כל מקרי השגיאה מחזירים את סוג התוצאה הנכון |
| 16 | LabelService | רשימה, יצירה, עדכון, מחיקה עם ניתוק מהמשימות | FN-MB-05, FN-AD-12 עד FN-AD-14 | כנ"ל |
| 17 | TaskService – ניהול | רשימה עם סינון, פרטים, יצירה, עדכון, מחיקה; בדיקת פרויקט ותוויות; LabelIds | FN-MB-03, FN-MB-04, FN-AD-09 עד FN-AD-11, FN-AD-03 | כנ"ל |
| 18 | TaskService – לקיחה | בדיקת Open, שמירה עם xmin, תפיסת חריגת Concurrency בלבד, Warning | FN-MB-06, ST-01 | תוצאת התנגשות במקום חריגה |
| 19 | TaskService – השלמה והתנגשות בעדכון | סימון Done ו-CompletedAt; רק מי שלקח; התנגשות xmin בעדכון | FN-MB-07, FN-AD-10, ST-01 | כל המעברים לפי ST-01 |
| 20 | UserService | רשימה, הוספה, עריכה, מחיקה; הגנה על Admin; משימות Taken | FN-AD-15 עד FN-AD-18, UM-01 עד UM-06 | כנ"ל |
| 21 | AuthService | הרשמה עם hash, התחברות עם הודעה אחידה; מחזיר פרטי משתמש בלי טוקן | FN-GU-01, FN-GU-02, RL-01 | Role תמיד Member בהרשמה |
| 22 | אינדקס ייחודי ומשתמש שנמחק | חריגת ייחודיות ← התנגשות; בדיקת קיום משתמש בכל פעולת כתיבה | GN-12, GN-09, UM-07 | אין 500 בכפילות מקבילה |

## מסמך 4 – API (משימות 23–28)

| # | משימה | פירוט | הפניות | סיום |
|---|---|---|---|---|
| 23 | Controllers: Projects, Labels | פעולות לפי מפת ה-endpoints | AP-01 עד AP-03 | כל הכתובות קיימות |
| 24 | Controller: Tasks | כולל assignee ו-completion; פרמטרי רשימה | AP-01, AP-02, GN-02 | כנ"ל |
| 25 | Controller: Users | ניהול משתמשים | AP-01 | כנ"ל |
| 26 | Controller: Auth | register ו-login; יצירת JWT אחרי הצלחת ה-Service | AP-04, GN-04 (מסמך 5) | התשובה כוללת Token, DisplayName, Role |
| 27 | המרת תוצאות לשגיאות | אובייקט תוצאה ← קוד HTTP ו-ProblemDetails עם title ו-correlationId | GN-03, GN-07, CO-05 | כל title לפי טבלת GN-07 |
| 28 | רישום DI | DbContext, Repositories, Services – Scoped; AutoMapper | AP-05 | CRUD מלא עובד מ-Swagger |

## מסמך 5 – תשתית ואבטחה (משימות 29–35)

| # | משימה | פירוט | הפניות | סיום |
|---|---|---|---|---|
| 29 | Configuration | appsettings, appsettings.Development, IOptions ל-JWT ול-CORS; קריאה ממשתני סביבה | IN-01, IN-02, GN-04-R2, DP-04-R1 | אין סוד ב-repo |
| 30 | Middleware שגיאות | תפיסה גלובלית, Error בלוג, 500 ב-ProblemDetails | GN-01 (MW-01), GN-03-R1 | חריגה מקרית מחזירה JSON אחיד |
| 31 | Middleware CorrelationId | מזהה לכל בקשה, בכותרת התשובה ובכל שורת לוג | GN-01 (MW-02) | שורת לוג לכל בקשה עם אותו מזהה |
| 32 | NLog | nlog.config, רמות, יעד Console ויעד קובץ בפיתוח, איסור רישום סודות | GN-06 | Warning בהתנגשות; אין סיסמה או טוקן בלוג |
| 33 | CORS | רק כתובת הלקוח, לפני Authentication | GN-08 | הלקוח מקבל תשובות; מקור אחר נחסם |
| 34 | Authentication ו-Authorization | וידוא JWT, Authorize לפי Role, סדר ה-pipeline | GN-01, GN-04, RL-01 עד RL-04 | בלי טוקן 401; Member בפעולת Admin 403 |
| 35 | Swagger עם Bearer | כפתור Authorize; פעיל בפיתוח בלבד | GN-10 | בדיקת endpoint מוגן מ-Swagger עובדת |

## מסמך 6 – בדיקות (משימות 36–40)

| # | משימה | פירוט | הפניות | סיום |
|---|---|---|---|---|
| 36 | תשתית בדיקות | xUnit, Moq, WebApplicationFactory, בסיס בדיקות שנוצר מחדש לפני כל הרצה | GN-05-R3 | בדיקת דמה רצה מול PostgreSQL |
| 37 | בדיקות Service | כל התרחישים ברמת Service | TC (רמה Service) | כולן עוברות |
| 38 | בדיקות Controller | GET, POST, PUT, DELETE לכל משאב מול Service מדומה | TC (רמה Controller) | כולן עוברות |
| 39 | בדיקות HTTP | בקשות מלאות כולל 401, 403, 404, 400, 409 | TC (רמה HTTP) | כולן עוברות |
| 40 | בדיקות תחרות | שני DbContext; שתי בקשות מקבילות; התנגשות בעדכון | TC-C-06, TC-C-07, TC-C-14 | הצלחה אחת ו-409 אחד |

## מסמך 7 – לקוח והגשה (משימות 41–45)

| # | משימה | פירוט | הפניות | סיום |
|---|---|---|---|---|
| 41 | בסיס React | מבנה תיקיות, ניתוב, שירות API עם טוקן, ניתוק ב-401, תפריט לפי Role; כתובת ה-API ממשתנה סביבה | SC-00, SC-01-R2, SC-01-R3, DP-04-R5 | ניווט לפי תפקיד עובד |
| 42 | התחברות והרשמה | SC-01, SC-02 עם הודעות לפי קוד | SC-01, SC-02 | התחברות ממשתמש Seed |
| 43 | משימות | רשימת משימות עם סינון ועמודים, לקיחה, המשימות שלי, השלמה | SC-03, SC-04 | שני דפדפנים: לקיחה שנייה מציגה הודעת 409 |
| 44 | מסכי ניהול | פרויקטים, תוויות, משתמשים | SC-05 עד SC-07, SC-00-R4 עד SC-00-R7 | כל פעולות המנהל עובדות מהמסכים |
| 45 | README והגשה | תוכן README כולל כתובות הפריסה; הרצה מ-clone נקי; בדיקה סופית (אחרי משימות 46–49) | RD-01 עד RD-07 | השרת והלקוח עולים במחשב נקי ובכתובת הציבורית |

## מסמך 5 – פריסה (משימות 46–49)

| # | משימה | פירוט | הפניות | סיום |
|---|---|---|---|---|
| 46 | Dockerfile | Dockerfile רב-שלבי, ‎.dockerignore, האזנה לפורט PORT | DP-01 | docker build עובר; הקונטיינר עולה ועונה לבקשה |
| 47 | docker-compose | שירותי db ו-api, ‎.env ו-‎.env.example, init.sql לבסיס הבדיקות | DP-02 | docker compose up מעלה את שניהם; ה-API מתחבר לבסיס |
| 48 | GitHub | דחיפת שני ה-repositories לענף main אחרי build ובדיקות | DP-03 | הקוד המלא ב-GitHub; אין סוד ב-repo |
| 49 | Render | PostgreSQL, Web Service מסוג Docker, Static Site ללקוח; משתני סביבה; Migrate בעלייה; פריסה אוטומטית | DP-04 | התחברות ממשתמש Seed דרך הכתובת הציבורית של הלקוח |
