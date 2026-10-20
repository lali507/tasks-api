# מערכת ניהול משימות

מערכת לניהול משימות בצוות: ASP.NET Core 8 Web API עם PostgreSQL, לקוח React נפרד, ופריסה ב-Docker וב-Render.

| | כתובת |
|---|---|
| **האתר (לקוח)** | https://tasks-client-3r5m.onrender.com |
| שרת ה-API | https://tasks-api-6f95.onrender.com |
| קוד השרת | https://github.com/lali507/tasks-api |
| קוד הלקוח | https://github.com/lali507/tasks-client |

## על המערכת (RD-01)

יש שני תפקידים. **Admin** מנהל פרויקטים, משימות, תוויות ומשתמשים. **Member** צופה במשימות, לוקח משימה פתוחה ומסמן משימה שלקח כהושלמה. משימה עוברת בין שלושה מצבים: Open ← Taken ← Done.

**המשאב המוגבל** הוא משימה פתוחה: רק משתמש אחד יכול לקחת אותה. כששני משתמשים לוקחים את אותה משימה באותו רגע, הבקשה הראשונה מצליחה והשנייה מקבלת **409 Conflict** ("המשימה כבר נלקחה"). המנגנון הוא Optimistic Concurrency של EF Core על עמודת המערכת `xmin` של PostgreSQL: העדכון נשמר רק אם השורה לא השתנתה מאז שנקראה. אחרת נזרקת `DbUpdateConcurrencyException`, היא נרשמת בלוג ברמת Warning, והתשובה היא 409. אותו מנגנון מגן גם על עריכה של משימה שמישהו אחר שינה בינתיים.

### מבנה

| פרויקט | תפקיד |
|---|---|
| `Task.Core` | ישויות, DTOs, ממשקי Repository ו-Service, אובייקט תוצאה. ללא תלויות |
| `Task.Data` | `DbContext`, הגדרות Fluent API, Migrations, Seed, Repositories |
| `Task.Service` | לוגיקה עסקית, ולידציה, מיפוי (Mapperly) |
| `Task.api` | Controllers, JWT, Middlewares (שגיאות, CorrelationId), CORS, Swagger, NLog |
| `Task.Tests` | xUnit: בדיקות Service, Controller, HTTP מלא מול PostgreSQL, ותחרות |

## הרצה מקומית (RD-02)

### דרישות
- [Docker Desktop](https://www.docker.com/products/docker-desktop/): בסיס הנתונים רץ רק בקונטיינר.
- .NET 8 SDK, ו-Node.js 22 (ללקוח).

### 1. קובץ ‎.env
בתיקיית השרת מעתיקים את `.env.example` ל-`.env` וממלאים ערכים:
```
POSTGRES_USER=postgres
POSTGRES_PASSWORD=<סיסמה>
POSTGRES_DB=TaskDb
POSTGRES_PORT=5432
JWT_KEY=<מחרוזת אקראית, לפחות 32 תווים>
CLIENT_URL=http://localhost:5173
API_PORT=8080
```
הקובץ `.env` לא נשמר ב-Git.

### 2א. הכול ב-Docker (הכי מהיר)
```bash
docker compose up -d --build
```
הפקודה מעלה את בסיס הנתונים ואת השרת בכתובת http://localhost:8080. בעלייה הראשונה השרת יוצר את הטבלאות וטוען את ה-Seed בעצמו (RD-06), בלי פעולה ידנית בבסיס הנתונים. השרת רץ במצב Production, ולכן Swagger כבוי.

### 2ב. השרת מ-Visual Studio / dotnet (לפיתוח)
מעלים רק את בסיס הנתונים:
```bash
docker compose up -d db
```
הסודות בפיתוח נשמרים ב-User Secrets ולא בקבצים:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=TaskDb;Username=postgres;Password=<סיסמה>" --project Task.api
dotnet user-secrets set "ConnectionStrings:TestConnection" "Host=localhost;Port=5432;Database=TaskDb_Test;Username=postgres;Password=<סיסמה>" --project Task.api
dotnet user-secrets set "Jwt:Key" "<מחרוזת אקראית, לפחות 32 תווים>" --project Task.api
```
עדכון בסיס הנתונים: השרת מריץ את ה-Migrations בעלייה. אפשר גם להריץ ידנית:
```bash
dotnet tool restore
dotnet ef database update --project Task.Data --startup-project Task.api
```
הרצה:
```bash
dotnet run --project Task.api
```
השרת עולה בכתובת http://localhost:5122. ב-Development יש גם Swagger, בכתובת http://localhost:5122/swagger: מתחברים, מעתיקים את הטוקן ולוחצים Authorize.

בדיקות. בסיס הבדיקות `TaskDb_Test` נוצר עם הקונטיינר, ונבנה מחדש לפני כל הרצה:
```bash
dotnet test Task.slnx
```

### 3. הלקוח
```bash
git clone https://github.com/lali507/tasks-client
cd tasks-client
npm install
```
יוצרים קובץ `.env.development` עם כתובת השרת:
- `VITE_API_URL=http://localhost:5122` אם השרת רץ מ-Visual Studio.
- `VITE_API_URL=http://localhost:8080` אם השרת רץ ב-Docker.

ואז מריצים:
```bash
npm run dev
```
הלקוח עולה בכתובת http://localhost:5173.

## משתמשי דמו (RD-03)

ה-Seed יוצר 30 משתמשים, 10 פרויקטים, 10 תוויות ו-100 משימות.

| משתמש | אימייל | סיסמה |
|---|---|---|
| מנהל (היחיד במערכת) | `admin@example.com` | `Demo1234!1` |
| חברי צוות | `member02@example.com` … `member30@example.com` | `Demo1234!2` … `Demo1234!30` |

הכלל: מספר הסיסמה זהה למספר שבאימייל. למשל `member07@example.com` / `Demo1234!7`.

## פריסה ב-Render (RD-07)

| רכיב | סוג ב-Render | מקור |
|---|---|---|
| בסיס נתונים | Render PostgreSQL 16 | – |
| שרת | Web Service מסוג Docker | המאגר `tasks-api`, `Dockerfile` |
| לקוח | Static Site | המאגר `tasks-client`, `npm ci && npm run build` ← `dist` |

**משתני סביבה בשרת** (במקום User Secrets):

| משתנה | ערך |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__DefaultConnection` | `Host=...;Port=5432;Database=...;Username=...;Password=...`, בפורמט Npgsql ולא בפורמט ה-URL ש-Render מציג. משתמשים בכתובת הפנימית |
| `Jwt__Key` | מחרוזת אקראית (Generate ב-Render) |
| `Cors__ClientUrl` | כתובת הלקוח, בלי `/` בסוף |

**בלקוח:** `VITE_API_URL` = כתובת השרת. בנוסף מוגדר כלל Rewrite: `/*` ← `/index.html`, כדי שרענון של דף פנימי לא יחזיר 404.

- השרת מאזין לפורט מהמשתנה `PORT` ש-Render מגדיר, ובלעדיו לפורט 8080.
- ה-Migrations וה-Seed רצים אוטומטית בעליית השרת.
- כל דחיפה ל-`main` נפרסת אוטומטית.
- השרת והלקוח הם שני שירותים נפרדים עם שתי כתובות. נכנסים תמיד דרך כתובת הלקוח.

## מגבלות ידועות (RD-04, RD-07)

- **אין אימות נוסף למנהל.** המנהל מתחבר באימייל וסיסמה כמו כל משתמש (RL-04).
- **רשימת הבחירה של פרויקטים** (בסינון ובטופס משימה) טוענת עד 50 פרויקטים (SC-00-R7).
- **משתמש שנמחק** עדיין יכול לצפות בנתונים עד שהטוקן שלו פג, לכל היותר 60 דקות. פעולות כתיבה נחסמות מיד (GN-09-R2).
- **תוכנית חינמית ב-Render:** השרת "נרדם" אחרי זמן ללא פעילות, והבקשה הראשונה אחרי זה יכולה לקחת עד כדקה (DP-04-R6).
- **בסיס הנתונים החינמי ב-Render נמחק אחרי 30 יום.** כדי להחזיר את המערכת, יוצרים בסיס חדש ומעדכנים את `ConnectionStrings__DefaultConnection`. השרת יבנה מחדש את הטבלאות ואת ה-Seed בעצמו.
- **כל החלונות של אותו דפדפן מחוברים לאותו משתמש**, כי הטוקן נשמר ב-localStorage. כשמתחברים כמשתמש אחר בחלון אחד, שאר החלונות נטענים מחדש עם המשתמש החדש. כדי לבדוק שני משתמשים בבת אחת, משתמשים בחלון גלישה בסתר או בדפדפן אחר.

## חריגות מכוונות מהאפיון (RD-05)

| כלל | באפיון | במימוש | למה |
|---|---|---|---|
| SC-00-R3 | ארבעה מסכים | נוספו מסכי ניהול למנהל: פרויקטים, תוויות, משתמשים (SC-05 עד SC-07) | בלעדיהם אפשר לנהל רק דרך ה-API |
| GN-04-R1 | הטוקן נוצר ב-Service | הטוקן נוצר בשכבת ה-API (`JwtTokenFactory`) | יצירת טוקן היא עניין של HTTP ושל ההגדרות, לא של לוגיקה עסקית |
| SV-01, AP-05 | AutoMapper Profiles | Mapperly (`Riok.Mapperly`), מיפוי סטטי ב-`Task.Service/Mapping/` | ל-AutoMapper החינמי (עד 14.0.0) יש חולשת אבטחה בדרגה High (GHSA-rvv3-g6hj-g44x), והגרסאות המתוקנות דורשות רישיון מסחרי |
