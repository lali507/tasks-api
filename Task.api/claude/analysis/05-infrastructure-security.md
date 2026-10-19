# מסמך ניתוח 5 – תשתית, אבטחה ופריסה

מערכת ניהול משימות .NET. שכבה: API – pipeline, הגדרות ופריסה. משמש את משימות הפיתוח 29–35 ו-46–49 (ראו dev-tasks.md). מזהים ממסמכים אחרים – לפי האינדקס (00-index.md).

### GN-01: Middlewares – לפי סדר ה-pipeline

| # | רכיב | סוג | תפקיד | פונקציות |
|---|---|---|---|---|
| MW-01 | ExceptionHandlingMiddleware | מותאם אישית | טיפול גלובלי בשגיאות | עוטף את כל השרשרת; תופס כל חריגה שלא טופלה; לוג Error עם CorrelationId; מחזיר 500 בתשובת JSON אחידה (ProblemDetails) בלי פרטים פנימיים |
| MW-02 | CorrelationIdMiddleware | מותאם אישית | מעקב אחרי בקשה | מזהה ייחודי לכל בקשה (או מכותרת הלקוח); מוכנס לכל שורת לוג; מוחזר בכותרת התשובה; לוג Information לכל בקשה (method, כתובת, קוד תשובה, משך) |
| MW-03 | Authentication | מובנה | זיהוי | קריאת הטוקן מ-Authorization: Bearer; וידוא חתימה ותוקף; מילוי claims |
| MW-04 | Authorization | מובנה | הרשאה | בדיקת Authorize ו-Role; 401 בלי זיהוי, 403 בלי הרשאה |

- סדר: שגיאות ← CorrelationId ← CORS (GN-08) ← Authentication ← Authorization.
- הפעלת Swagger בסביבת פיתוח בלבד.
- אין רישום ללוג של סיסמאות, טוקנים או גוף בקשת הרשמה/התחברות/הוספת משתמש/עריכת משתמש.

### GN-04: טוקן

| נושא | ערך |
|---|---|
| שכבה יוצרת | שכבת ה-API (ראו GN-04-R1) |
| סוג | JWT |
| תוקף | 60 דקות |
| Refresh Token | אין – מחוץ לתחום |
| Claims | מזהה משתמש (sub), Role, Email |
| אלגוריתם חתימה | HMAC-SHA256 |
| מפתח החתימה | User Secrets בפיתוח; משתנה סביבה בפריסה (DP-04-R1); לעולם לא בקוד או ב-repo |
| שליחה מהלקוח | כותרת Authorization: Bearer (MW-03) |
| תפוגה | הבקשה הראשונה אחרי 60 דקות ← 401; הלקוח מנתק (SC-01-R3) |

- כלל GN-04-R1: חריגה מכוונת מהאפיון – האפיון קובע שהטוקן נוצר ב-Service; בהחלטת המשתמש הוא נוצר בשכבת ה-API. ה-Service מאמת את המשתמש ומחזיר את פרטיו, וה-API יוצר מהם את הטוקן. (עמימות 19)
- כלל GN-04-R2: הגדרות הטוקן (תוקף, מנפיק, קהל) נקראות מ-appsettings דרך IOptions; המפתח בלבד ב-User Secrets או במשתנה סביבה.

### GN-06: לוגים

| מצב | רמת לוג | תוכן |
|---|---|---|
| כל בקשה | Information | method, כתובת, קוד תשובה, משך, CorrelationId |
| 409 עסקי (בדיקה ב-Service, בלי התנגשות) | – | רק לוג הבקשה הרגיל |
| התנגשות Concurrency (xmin) | Warning | CorrelationId, מזהה המשימה, הפעולה |
| חריגה שלא טופלה (500) | Error | CorrelationId, סוג החריגה |
| פרטי פיתוח | Debug | בסביבת פיתוח בלבד |

- כלל GN-06-R1: אסור לרשום ללוג סיסמאות, טוקנים או גוף בקשות הרשמה, התחברות, הוספת משתמש ועריכת משתמש (GN-01).
- כלל GN-06-R2: NLog עם הגדרות בקובץ nlog.config; הקוד משתמש ב-ILogger הרגיל.
- כלל GN-06-R3: בהתנגשות בלקיחה – בקשה אחת מקבלת 200 והשנייה 409, ונרשם Warning אחד (FN-MB-06).
- כלל GN-06-R4: יעד Console תמיד פעיל, כדי שהלוגים יוצגו ב-Docker וב-Render; יעד קובץ בסביבת פיתוח בלבד.

### GN-08: CORS
- כלל GN-08-R1: השרת מתיר בקשות רק מכתובת לקוח ה-React; אין היתר לכל המקורות (AllowAnyOrigin).
- כלל GN-08-R2: כתובת הלקוח נשמרת ב-appsettings (בפיתוח – appsettings.Development.json; בפריסה – משתנה סביבה, DP-04-R1), לא בקוד.
- כלל GN-08-R3: מותרות כל ה-methods (GET, POST, PUT, DELETE) והכותרת Authorization.
- כלל GN-08-R4: CORS ממוקם ב-pipeline לפני Authentication, כדי שבקשת OPTIONS מקדימה בלי טוקן תעבור (GN-01).

### GN-10: Swagger
- כלל GN-10-R1: Swagger פעיל בסביבת פיתוח בלבד (GN-01); כבוי בפריסה ב-Render.
- כלל GN-10-R2: Swagger מוגדר עם Bearer – כפתור Authorize שמאפשר להזין טוקן ולבדוק endpoints מוגנים.

## Configuration
- כלל IN-01: appsettings.json בלי סודות; appsettings.Development.json לסביבת פיתוח; Connection String ומפתח JWT ב-User Secrets בפיתוח, ובמשתני סביבה בפריסה (DP-04-R1).
- כלל IN-02: הגדרות JWT ו-CORS נקראות דרך IOptions.
- כלל IN-03: PostgreSQL בפיתוח רץ בקונטיינר Docker (DP-02), ולא בהתקנה מקומית.

## Docker ופריסה

### DP-01: Dockerfile לשרת
- כלל DP-01-R1: Dockerfile רב-שלבי בתיקיית ה-Solution: שלב בנייה עם תמונת ה-SDK, ושלב הרצה עם תמונת ASP.NET Runtime בלבד.
- כלל DP-01-R2: פרויקט הבדיקות אינו נכלל בתמונת ההרצה.
- כלל DP-01-R3: קובץ ‎.dockerignore מחריג bin, obj, ‎.env וקבצי סודות.
- כלל DP-01-R4: השרת מאזין לפורט שמגיע ממשתנה הסביבה PORT, עם ברירת מחדל 8080.

### DP-02: docker-compose לפיתוח מקומי

| שירות | תמונה | תפקיד |
|---|---|---|
| db | postgres:16 | בסיס הנתונים; volume לשמירת נתונים; init.sql יוצר את בסיס הבדיקות TaskDb_Test |
| api | נבנה מה-Dockerfile (DP-01) | השרת; תלוי ב-db; מקבל מחרוזת חיבור ומפתח JWT מ-‎.env |

- כלל DP-02-R1: סיסמאות בקובץ ‎.env שאינו עולה ל-GitHub; קובץ ‎.env.example עם אותו מבנה בלי ערכים – עולה.
- כלל DP-02-R2: הרצה מה-IDE ממשיכה לעבוד מול שירות ה-db בקונטיינר, עם User Secrets.

### DP-03: GitHub
- כלל DP-03-R1: שני repositories (משימה 1); ענף main הוא ענף הפריסה.
- כלל DP-03-R2: לפני דחיפה ל-main – ה-Solution מתקמפל וכל הבדיקות עוברות מקומית.
- כלל DP-03-R3: אין סודות ב-repo (IN-01); ‎.env ו-User Secrets מחוץ ל-repo.

### DP-04: Render

| רכיב | סוג ב-Render | מקור |
|---|---|---|
| בסיס נתונים | Render PostgreSQL | – |
| שרת API | Web Service מסוג Docker | repository השרת, ה-Dockerfile (DP-01) |
| לקוח React | Static Site | repository הלקוח, פקודת build של React |

- כלל DP-04-R1: משתני סביבה במקום User Secrets: ConnectionStrings__DefaultConnection, Jwt__Key, Cors__ClientUrl, ASPNETCORE_ENVIRONMENT=Production.
- כלל DP-04-R2: מחרוזת החיבור נכתבת בפורמט Key=Value של Npgsql (Host=...;Database=...;Username=...;Password=...), ולא בפורמט ה-URL ש-Render מציג.
- כלל DP-04-R3: ה-Migrations, כולל ה-Seed, מוחלות אוטומטית בעליית השרת (Database.Migrate), בהתאם ל-RD-06.
- כלל DP-04-R4: פריסה אוטומטית בכל דחיפה ל-main (DP-03-R1).
- כלל DP-04-R5: כתובת ה-API בלקוח נקראת ממשתנה סביבה של ה-build; כתובת הלקוח ב-Render נכנסת ל-Cors__ClientUrl (GN-08-R2).
- כלל DP-04-R6: בתוכנית החינמית השירות נרדם אחרי חוסר פעילות, והבקשה הראשונה איטית – מגבלה ידועה ב-README (RD-07).
- כלל DP-04-R7: השרת והלקוח נפרסים כשני שירותים נפרדים עם שתי כתובות; המשתמש נכנס דרך כתובת הלקוח בלבד, והלקוח פונה לכתובת ה-API. לא מגישים את הלקוח מתוך השרת. (OP-06)
