# מסמך ניתוח 6 – תרחישי בדיקה

מערכת ניהול משימות .NET. שכבה: Tests. משמש את משימות הפיתוח 36–40 (ראו dev-tasks.md). מזהים ממסמכים אחרים – לפי האינדקס (00-index.md).

## תרחישי בדיקה

רמות: Service – לוגיקה עם Repository מדומה; Controller – המרת תוצאה לקוד HTTP מול Service מדומה; HTTP – WebApplicationFactory מול PostgreSQL לבדיקות. ה-title הצפוי בכל שגיאה – לפי GN-07.

### TC-A: Auth

| מזהה | פונקציה | תנאי קדם וקלט | תוצאה צפויה | רמה |
|---|---|---|---|---|
| TC-A-01 | FN-GU-01 | נתונים תקינים | 201, AuthResponse עם Token, DisplayName, Role = Member | HTTP |
| TC-A-02 | FN-GU-01 | אימייל קיים (גם באותיות גדולות) | 409 "האימייל כבר רשום" | Service + HTTP |
| TC-A-03 | FN-GU-01 | סיסמה של 7 תווים / בלי ספרה | 400 "הנתונים שנשלחו אינם תקינים" + errors.Password | Controller + HTTP |
| TC-A-04 | FN-GU-01 | DisplayName של רווחים בלבד | 400 "הנתונים שנשלחו אינם תקינים" (אחרי Trim) | Service |
| TC-A-05 | FN-GU-01 | אימייל עם רווחים ואותיות גדולות | נשמר אחרי Trim ובאותיות קטנות | Service |
| TC-A-06 | FN-GU-01 | שדה Role = Admin נשלח בגוף | 201, Role = Member | HTTP |
| TC-A-07 | FN-GU-02 | פרטי Seed תקינים | 200, Token עם claims sub, Role, Email | HTTP |
| TC-A-08 | FN-GU-02 | סיסמה שגויה | 401 "אימייל או סיסמה שגויים" | Service + HTTP |
| TC-A-09 | FN-GU-02 | אימייל לא קיים | 401 "אימייל או סיסמה שגויים" (זהה ל-TC-A-08) | Service + HTTP |
| TC-A-10 | FN-GU-02 | שדה חסר | 400 "הנתונים שנשלחו אינם תקינים" | Controller |
| TC-A-11 | GN-04 | טוקן שפג תוקפו בבקשה מוגנת | 401 "נדרשת התחברות" | HTTP |
| TC-A-12 | GN-06 | הרשמה תקינה | הלוג אינו מכיל סיסמה או טוקן | HTTP |
| TC-A-13 | GN-12 | Service מקבל חריגת ייחודיות מ-Repository מדומה בהרשמה | 409 "האימייל כבר רשום", לא 500 | Service |

### TC-P: Projects

| מזהה | פונקציה | תנאי קדם וקלט | תוצאה צפויה | רמה |
|---|---|---|---|---|
| TC-P-01 | FN-MB-01 | Member, ברירת מחדל | 200, עמוד 1 של 10, ממוין לפי שם, TotalCount = 10 | HTTP |
| TC-P-02 | FN-MB-01 | pageSize = 51 / page = 0 | 400 "פרמטר לא תקין" | Controller + HTTP |
| TC-P-03 | FN-MB-01 | page גדול מהעמוד האחרון | 200, רשימה ריקה, TotalCount נכון | HTTP |
| TC-P-04 | FN-MB-01 | בלי טוקן | 401 "נדרשת התחברות" | HTTP |
| TC-P-05 | FN-MB-02 | פרויקט קיים | 200, משימות עם תוויות, ממוינות לפי CreatedAt | HTTP |
| TC-P-06 | FN-MB-02 | מזהה לא קיים | 404 "לא נמצא" | Controller + HTTP |
| TC-P-07 | FN-AD-06 | Admin, נתונים תקינים | 201, Location, TaskCount = 0 | HTTP |
| TC-P-08 | FN-AD-06 | שם קיים באותיות שונות | 409 "שם הפרויקט כבר קיים" | Service + HTTP |
| TC-P-09 | FN-AD-06 | שם של תו אחד אחרי Trim | 400 "הנתונים שנשלחו אינם תקינים" | Service |
| TC-P-10 | FN-AD-06 | Member שולח POST | 403 "אין לך הרשאה לפעולה זו" | HTTP |
| TC-P-11 | FN-AD-07 | עדכון עם אותו שם לאותו פרויקט | 200 | Service |
| TC-P-12 | FN-AD-07 | שם של פרויקט אחר | 409 "שם הפרויקט כבר קיים" | Service + HTTP |
| TC-P-13 | FN-AD-07 | מזהה לא קיים | 404 "לא נמצא" | Controller + HTTP |
| TC-P-14 | FN-AD-08 | פרויקט בלי משימות | 204 | HTTP |
| TC-P-15 | FN-AD-08 | פרויקט עם משימות | 409 "לא ניתן למחוק פרויקט שיש בו משימות" | Service + HTTP |
| TC-P-16 | FN-AD-08 | Member שולח DELETE | 403 "אין לך הרשאה לפעולה זו" | HTTP |

### TC-L: Labels

| מזהה | פונקציה | תנאי קדם וקלט | תוצאה צפויה | רמה |
|---|---|---|---|---|
| TC-L-01 | FN-MB-05 | Member | 200, כל 10 התוויות ממוינות לפי שם, בלי PagedResponse | HTTP |
| TC-L-02 | FN-MB-05 | בלי טוקן | 401 "נדרשת התחברות" | HTTP |
| TC-L-03 | FN-AD-12 | Admin, שם תקין | 201, Location, LabelResponse | HTTP |
| TC-L-04 | FN-AD-12 | שם קיים באותיות שונות | 409 "שם התווית כבר קיים" | Service + HTTP |
| TC-L-05 | FN-AD-12 | שם של 31 תווים | 400 "הנתונים שנשלחו אינם תקינים" + errors.Name | Controller + HTTP |
| TC-L-06 | FN-AD-12 | Member שולח POST | 403 "אין לך הרשאה לפעולה זו" | HTTP |
| TC-L-07 | FN-AD-13 | עדכון שם תקין | 200, השם החדש מופיע במשימות המשויכות | HTTP |
| TC-L-08 | FN-AD-13 | שם של תווית אחרת | 409 "שם התווית כבר קיים" | Service + HTTP |
| TC-L-09 | FN-AD-13 | מזהה לא קיים | 404 "לא נמצא" | Controller + HTTP |
| TC-L-10 | FN-AD-14 | תווית משויכת למשימות | 204, המשימות קיימות בלי התווית | HTTP |
| TC-L-11 | FN-AD-14 | מזהה לא קיים | 404 "לא נמצא" | Controller + HTTP |
| TC-L-12 | FN-AD-14 | Member שולח DELETE | 403 "אין לך הרשאה לפעולה זו" | HTTP |

### TC-T: Tasks – צפייה וניהול

| מזהה | פונקציה | תנאי קדם וקלט | תוצאה צפויה | רמה |
|---|---|---|---|---|
| TC-T-01 | FN-MB-03 | Member, ברירת מחדל | 200, 10 משימות, החדשה ראשונה, TotalCount = 100 | HTTP |
| TC-T-02 | FN-MB-03 | status=Open&projectId=1 | 200, רק משימות Open של פרויקט 1 | HTTP |
| TC-T-03 | FN-MB-03 | status=Closed | 400 "פרמטר לא תקין" | Controller + HTTP |
| TC-T-04 | FN-MB-03 | projectId שאינו קיים | 200, רשימה ריקה | HTTP |
| TC-T-05 | FN-MB-03 | Member עם mine=true | 200, רק משימות שהוא לקח | HTTP |
| TC-T-06 | FN-AD-03 | Admin עם mine=true | 200, רשימה ריקה | HTTP |
| TC-T-07 | FN-MB-04 | משימה קיימת | 200, TaskResponse עם תוויות, בלי xmin | HTTP |
| TC-T-08 | FN-MB-04 | מזהה לא קיים | 404 "לא נמצא" | Controller + HTTP |
| TC-T-09 | FN-AD-09 | Admin, נתונים תקינים ותוויות | 201, Location, Status = Open | HTTP |
| TC-T-10 | FN-AD-09 | ProjectId לא קיים | 400 "הנתונים שנשלחו אינם תקינים" + errors.ProjectId | Service + HTTP |
| TC-T-11 | FN-AD-09 | LabelIds עם מזהה לא קיים | 400 "הנתונים שנשלחו אינם תקינים" + errors.LabelIds | Service |
| TC-T-12 | FN-AD-09 | LabelIds עם כפילות | 201, כל תווית פעם אחת | Service |
| TC-T-13 | FN-AD-09 | Member שולח POST | 403 "אין לך הרשאה לפעולה זו" | HTTP |
| TC-T-14 | FN-AD-10 | משימה Taken, נתונים תקינים | 200, Status ו-TakenByUserId ללא שינוי | Service + HTTP |
| TC-T-15 | FN-AD-10 | LabelIds חדשה | 200, התוויות הוחלפו כולן | HTTP |
| TC-T-16 | FN-AD-10 | משימה Done | 409 "לא ניתן לעדכן משימה שהושלמה" | Service + HTTP |
| TC-T-17 | FN-AD-10 | מזהה לא קיים | 404 "לא נמצא" | Controller + HTTP |
| TC-T-18 | FN-AD-11 | משימה Open / Done | 204 | HTTP |
| TC-T-19 | FN-AD-11 | משימה Taken | 409 "לא ניתן למחוק משימה שנלקחה" | Service + HTTP |
| TC-T-20 | FN-AD-11 | מזהה לא קיים | 404 "לא נמצא" | Controller + HTTP |

### TC-C: לקיחה, השלמה ותחרות

| מזהה | פונקציה | תנאי קדם וקלט | תוצאה צפויה | רמה |
|---|---|---|---|---|
| TC-C-01 | FN-MB-06 | Member, משימה Open | 200, Status = Taken, TakenByUserId = המשתמש | Service + HTTP |
| TC-C-02 | FN-MB-06 | משימה Taken / Done | 409 "המשימה כבר נלקחה" | Service + HTTP |
| TC-C-03 | FN-MB-06 | Admin שולח PUT | 403 "אין לך הרשאה לפעולה זו" | HTTP |
| TC-C-04 | FN-MB-06 | מזהה לא קיים | 404 "לא נמצא" | Controller + HTTP |
| TC-C-05 | FN-MB-06 | בלי טוקן | 401 "נדרשת התחברות" | HTTP |
| TC-C-06 | FN-MB-06 | שני DbContext קוראים אותה משימה, שניהם שומרים | הראשון מצליח, השני מקבל חריגת Concurrency | PostgreSQL אמיתי |
| TC-C-07 | FN-MB-06 | שתי בקשות HTTP מקבילות משני Members | 200 אחד, 409 "המשימה כבר נלקחה" אחד, Warning אחד בלוג | HTTP |
| TC-C-08 | FN-MB-06 | Service מקבל חריגת Concurrency מ-Repository מדומה | תוצאת התנגשות ← 409 | Service |
| TC-C-09 | FN-MB-07 | מי שלקח, משימה Taken | 200, Status = Done, CompletedAt ממולא | Service + HTTP |
| TC-C-10 | FN-MB-07 | Member אחר | 403 "רק מי שלקח יכול לסמן" | Service + HTTP |
| TC-C-11 | FN-MB-07 | משימה Open / Done | 409 "ניתן לסמן רק משימה שנלקחה" | Service + HTTP |
| TC-C-12 | FN-MB-07 | Admin שולח PUT | 403 "אין לך הרשאה לפעולה זו" | HTTP |
| TC-C-13 | FN-MB-07 | מזהה לא קיים | 404 "לא נמצא" | Controller + HTTP |
| TC-C-14 | FN-AD-10 | Admin מעדכן משימה שנלקחה בינתיים | 409 "המשימה השתנתה, טען מחדש", Warning בלוג | PostgreSQL אמיתי |

### TC-U: Users

| מזהה | פונקציה | תנאי קדם וקלט | תוצאה צפויה | רמה |
|---|---|---|---|---|
| TC-U-01 | FN-AD-15 | Admin, ברירת מחדל | 200, 10 משתמשים ממוינים לפי DisplayName, TotalCount = 30, בלי PasswordHash | HTTP |
| TC-U-02 | FN-AD-15 | Member שולח GET | 403 "אין לך הרשאה לפעולה זו" | HTTP |
| TC-U-03 | FN-AD-16 | נתונים תקינים | 201, Location, Role = Member, בלי טוקן | HTTP |
| TC-U-04 | FN-AD-16 | אימייל קיים | 409 "האימייל כבר רשום" | Service + HTTP |
| TC-U-05 | FN-AD-16 | סיסמה לא תקינה | 400 "הנתונים שנשלחו אינם תקינים" + errors.Password | Controller + HTTP |
| TC-U-06 | FN-AD-17 | עדכון בלי NewPassword | 200, הסיסמה הישנה עדיין עובדת בהתחברות | Service + HTTP |
| TC-U-07 | FN-AD-17 | עדכון עם NewPassword | 200, רק הסיסמה החדשה עובדת | HTTP |
| TC-U-08 | FN-AD-17 | אימייל של משתמש אחר | 409 "האימייל כבר רשום" | Service + HTTP |
| TC-U-09 | FN-AD-17 | עריכת Admin | 403 "לא ניתן לשנות משתמש מנהל" | Service + HTTP |
| TC-U-10 | FN-AD-17 | מזהה לא קיים | 404 "לא נמצא" | Controller + HTTP |
| TC-U-11 | FN-AD-18 | משתמש בלי משימות | 204 | HTTP |
| TC-U-12 | FN-AD-18 | כל משימותיו Done | 204, המשימות Done עם TakenByUserId = null | HTTP |
| TC-U-13 | FN-AD-18 | יש לו משימה Taken | 409 "למשתמש יש משימה פעילה" | Service + HTTP |
| TC-U-14 | FN-AD-18 | מחיקת Admin | 403 "לא ניתן לשנות משתמש מנהל" | Service + HTTP |
| TC-U-15 | FN-AD-18 | מזהה לא קיים | 404 "לא נמצא" | Controller + HTTP |
| TC-U-16 | UM-07 | טוקן של משתמש שנמחק, פעולת כתיבה | 401 "נדרשת התחברות" | HTTP |

## טבלת כיסוי – פריט ← תרחישים

| פריט | תרחישים |
|---|---|
| FN-GU-01 | TC-A-01, TC-A-02, TC-A-03, TC-A-04, TC-A-05, TC-A-06 |
| FN-GU-02 | TC-A-07, TC-A-08, TC-A-09, TC-A-10 |
| FN-MB-01 | TC-P-01, TC-P-02, TC-P-03, TC-P-04 |
| FN-MB-02 | TC-P-05, TC-P-06 |
| FN-MB-03 | TC-T-01, TC-T-02, TC-T-03, TC-T-04, TC-T-05 |
| FN-MB-04 | TC-T-07, TC-T-08 |
| FN-MB-05 | TC-L-01, TC-L-02 |
| FN-MB-06 | TC-C-01, TC-C-02, TC-C-03, TC-C-04, TC-C-05, TC-C-06, TC-C-07, TC-C-08 |
| FN-MB-07 | TC-C-09, TC-C-10, TC-C-11, TC-C-12, TC-C-13 |
| FN-AD-03 | TC-T-06 |
| FN-AD-06 | TC-P-07, TC-P-08, TC-P-09, TC-P-10 |
| FN-AD-07 | TC-P-11, TC-P-12, TC-P-13 |
| FN-AD-08 | TC-P-14, TC-P-15, TC-P-16 |
| FN-AD-09 | TC-T-09, TC-T-10, TC-T-11, TC-T-12, TC-T-13 |
| FN-AD-10 | TC-T-14, TC-T-15, TC-T-16, TC-T-17, TC-C-14 |
| FN-AD-11 | TC-T-18, TC-T-19, TC-T-20 |
| FN-AD-12 | TC-L-03, TC-L-04, TC-L-05, TC-L-06 |
| FN-AD-13 | TC-L-07, TC-L-08, TC-L-09 |
| FN-AD-14 | TC-L-10, TC-L-11, TC-L-12 |
| FN-AD-15 | TC-U-01, TC-U-02 |
| FN-AD-16 | TC-U-03, TC-U-04, TC-U-05 |
| FN-AD-17 | TC-U-06, TC-U-07, TC-U-08, TC-U-09, TC-U-10 |
| FN-AD-18 | TC-U-11, TC-U-12, TC-U-13, TC-U-14, TC-U-15 |
| GN-04 | TC-A-11 |
| GN-06 | TC-A-12 |
| GN-12 | TC-A-13 |
| UM-07 | TC-U-16 |

- כלל TS-01: פונקציות צפייה של Admin הזהות לפונקציות Member (FN-AD-01, 02, 04, 05) מכוסות דרך תרחישי ה-Member המקבילים.
