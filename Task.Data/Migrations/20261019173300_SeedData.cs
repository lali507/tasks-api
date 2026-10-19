using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Labels",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "באג" },
                    { 2, "פיצ'ר" },
                    { 3, "דחוף" },
                    { 4, "עיצוב" },
                    { 5, "שרת" },
                    { 6, "לקוח" },
                    { 7, "בדיקות" },
                    { 8, "תיעוד" },
                    { 9, "ביצועים" },
                    { 10, "אבטחה" }
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "בניית אתר תדמית חדש לחברה", "אתר החברה" },
                    { 2, new DateTime(2026, 1, 2, 9, 0, 0, 0, DateTimeKind.Utc), "אפליקציה לאנדרואיד ול-iOS עבור לקוחות", "אפליקציית מובייל" },
                    { 3, new DateTime(2026, 1, 2, 10, 0, 0, 0, DateTimeKind.Utc), "ניהול קשרי לקוחות ותהליכי מכירה", "מערכת CRM" },
                    { 4, new DateTime(2026, 1, 2, 11, 0, 0, 0, DateTimeKind.Utc), "חנות אונליין עם תשלום מאובטח", "חנות מקוונת" },
                    { 5, new DateTime(2026, 1, 2, 12, 0, 0, 0, DateTimeKind.Utc), "חישוב שכר והפקת דוחות לעובדים", "מערכת שכר" },
                    { 6, new DateTime(2026, 1, 2, 13, 0, 0, 0, DateTimeKind.Utc), "פורטל פנימי למידע, הודעות וטפסים", "פורטל עובדים" },
                    { 7, new DateTime(2026, 1, 2, 14, 0, 0, 0, DateTimeKind.Utc), "ניהול מלאי, ספקים ומחסנים", "מערכת מלאי" },
                    { 8, new DateTime(2026, 1, 2, 15, 0, 0, 0, DateTimeKind.Utc), "לוחות מחוונים ודוחות להנהלה", "דוחות ניהול" },
                    { 9, new DateTime(2026, 1, 2, 16, 0, 0, 0, DateTimeKind.Utc), "הזמנת תורים ושירותים אונליין", "מערכת הזמנות" },
                    { 10, new DateTime(2026, 1, 2, 17, 0, 0, 0, DateTimeKind.Utc), "העברת השרתים לענן והקמת ניטור", "תשתית ענן" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DisplayName", "Email", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc), "מנהל המערכת", "admin@example.com", "AQAAAAIAAYagAAAAEE5k5ddjrJetqI1M2qMV43Wk4vJDwu/RKylBc4ibPQmou8GEspyUaQPnl4/YkPYmSw==", "Admin" },
                    { 2, new DateTime(2026, 1, 1, 8, 1, 0, 0, DateTimeKind.Utc), "דנה כהן", "member02@example.com", "AQAAAAIAAYagAAAAEBHsijZrUYxG3pey5UOPMPESjxtVleFPJgljOaHJoY2StfjQYAzhnkILH8aavu1sEQ==", "Member" },
                    { 3, new DateTime(2026, 1, 1, 8, 2, 0, 0, DateTimeKind.Utc), "יוסי לוי", "member03@example.com", "AQAAAAIAAYagAAAAEF5LUmT4pouBsnlQlSP/865LPeU2WKgzIO7JHbGVWfGmqyd+wpjBpjMduZevEmlWjg==", "Member" },
                    { 4, new DateTime(2026, 1, 1, 8, 3, 0, 0, DateTimeKind.Utc), "מיכל אברהם", "member04@example.com", "AQAAAAIAAYagAAAAEC3JMflVxJFleAfDNJgSEMoopxrZLOdsZWEf+M4wV3iVPWyiysjBfiqtxf0Md8yhGA==", "Member" },
                    { 5, new DateTime(2026, 1, 1, 8, 4, 0, 0, DateTimeKind.Utc), "אבי מזרחי", "member05@example.com", "AQAAAAIAAYagAAAAEDa9w1TCSQ2AWn2MFaWapxQTRUngo4q6Y4lqcYnso2QquhZKWMZEp9k4nJy9YFPe7Q==", "Member" },
                    { 6, new DateTime(2026, 1, 1, 8, 5, 0, 0, DateTimeKind.Utc), "נועה פרץ", "member06@example.com", "AQAAAAIAAYagAAAAEPyybscE1G9LPfCR5ttVL8pEl3LvvWgzj9CKLdyjWW3pwOsnRoA1cp+auipPoY9+5w==", "Member" },
                    { 7, new DateTime(2026, 1, 1, 8, 6, 0, 0, DateTimeKind.Utc), "רון ביטון", "member07@example.com", "AQAAAAIAAYagAAAAEJm2QmwN58wi+wV+P2xXQ94DfwLXkoWAFDtBqFuwVgGRvGwPVyFFoG6zO2H13P7EOg==", "Member" },
                    { 8, new DateTime(2026, 1, 1, 8, 7, 0, 0, DateTimeKind.Utc), "שירה דהן", "member08@example.com", "AQAAAAIAAYagAAAAEM0MOy/qQWruNEYi7pd1CxXIjnWP8GtO5hcvfn4yzWmRtWW9g4MTmiJjhO97IF1y0g==", "Member" },
                    { 9, new DateTime(2026, 1, 1, 8, 8, 0, 0, DateTimeKind.Utc), "עומר אזולאי", "member09@example.com", "AQAAAAIAAYagAAAAEN8gkrDWynoYgq5g3N7dZdhHQSFrH5HMYHreuYj5wCD+VygXgcvzgtZOlH0dfcgfeA==", "Member" },
                    { 10, new DateTime(2026, 1, 1, 8, 9, 0, 0, DateTimeKind.Utc), "תמר פרידמן", "member10@example.com", "AQAAAAIAAYagAAAAEBiP8dHXlSn7NLQapVYGyzfrxvgEnbY4mE1d2PIWQO10JrTL3sDhHu6ZzxcJ1Tlciw==", "Member" },
                    { 11, new DateTime(2026, 1, 1, 8, 10, 0, 0, DateTimeKind.Utc), "איתי שפירא", "member11@example.com", "AQAAAAIAAYagAAAAEKzjBuYl5A33dbnI6oXK+X5gyLAeq3uZifuNkMqbij6TyRZAaCyKRPWSSrh5+ZQRyw==", "Member" },
                    { 12, new DateTime(2026, 1, 1, 8, 11, 0, 0, DateTimeKind.Utc), "יעל גולן", "member12@example.com", "AQAAAAIAAYagAAAAEBVp3k27qTmc/K1MaVF7H3p+EsYa19LgMw2v3Jb58JoSBIcOb9Qk5/x4c8ZIpyG80w==", "Member" },
                    { 13, new DateTime(2026, 1, 1, 8, 12, 0, 0, DateTimeKind.Utc), "גיא אוחיון", "member13@example.com", "AQAAAAIAAYagAAAAEAW1tPIltG+UtP3ocTxoaQlkygREI8Ia0EiQdQ7KGxozThcENXaSqXvmZ+O+M7gpvQ==", "Member" },
                    { 14, new DateTime(2026, 1, 1, 8, 13, 0, 0, DateTimeKind.Utc), "ליאת חדד", "member14@example.com", "AQAAAAIAAYagAAAAEJIyu8oEcObM9NXqImUph4SRHVhvklKCCZHZ7VGgyOU55pLPs/URMEqS5qQ37nx8ZQ==", "Member" },
                    { 15, new DateTime(2026, 1, 1, 8, 14, 0, 0, DateTimeKind.Utc), "אלון סויסה", "member15@example.com", "AQAAAAIAAYagAAAAEDcJoFA1JamAT+H5xqn04tpenxT+Dw0RIZ58FLn9LOwfG9onIy5cgJnocKr4sDoI4A==", "Member" },
                    { 16, new DateTime(2026, 1, 1, 8, 15, 0, 0, DateTimeKind.Utc), "מאיה רוזן", "member16@example.com", "AQAAAAIAAYagAAAAEOdW5vyqolh8JD2GCxKo7/aOalRshF5FKJcy1Jufy+JbzjcohFRo2hhlPp23jEkMCg==", "Member" },
                    { 17, new DateTime(2026, 1, 1, 8, 16, 0, 0, DateTimeKind.Utc), "דוד קליין", "member17@example.com", "AQAAAAIAAYagAAAAEAOa7OWYfm6acCAPmB3HGP3dbXSYvqIU0GXPyfhtvjyUJktv/AvIXU4XQNohU/e79g==", "Member" },
                    { 18, new DateTime(2026, 1, 1, 8, 17, 0, 0, DateTimeKind.Utc), "רותם בן דוד", "member18@example.com", "AQAAAAIAAYagAAAAEOmcjb88bBkoMeWnxbj2kYN44gm4K2N99fyE5G+a/TlE0/1+CFghDAZ0ba9ENlkdOA==", "Member" },
                    { 19, new DateTime(2026, 1, 1, 8, 18, 0, 0, DateTimeKind.Utc), "אורי וקנין", "member19@example.com", "AQAAAAIAAYagAAAAEPXrh0HMH+dQ6JmfTIUsMD259Nw0mvRdyIRNSDER5wYYKCrcHxc82SEqrxtpjFVNIw==", "Member" },
                    { 20, new DateTime(2026, 1, 1, 8, 19, 0, 0, DateTimeKind.Utc), "הילה אשכנזי", "member20@example.com", "AQAAAAIAAYagAAAAEC5iwDlgBk1yqbP64a30e0aFZkmh2BQykJsEYw4yaDeaXZfpvN2eISVOnTpwWOVRXw==", "Member" },
                    { 21, new DateTime(2026, 1, 1, 8, 20, 0, 0, DateTimeKind.Utc), "ניר גבאי", "member21@example.com", "AQAAAAIAAYagAAAAEDGtNyX6UcUTqyoxSIVvTZN7PfabJY14RCswv329Mf4Q79czgx67ouSgYt+bdXwcbw==", "Member" },
                    { 22, new DateTime(2026, 1, 1, 8, 21, 0, 0, DateTimeKind.Utc), "שני טל", "member22@example.com", "AQAAAAIAAYagAAAAEILPYwH92N4yh29FriguXtLeoOHOE9PLjB/5QyZaSTP54N4vEcj+fIFajD2fs2/2bg==", "Member" },
                    { 23, new DateTime(2026, 1, 1, 8, 22, 0, 0, DateTimeKind.Utc), "עידו כץ", "member23@example.com", "AQAAAAIAAYagAAAAEBC5wyg8e5cJNT/Fx1v4AI37wopFBUHwiRpkOe0pZ16ekZ9jiV91yvU01J/RHhXyYQ==", "Member" },
                    { 24, new DateTime(2026, 1, 1, 8, 23, 0, 0, DateTimeKind.Utc), "אסנת יוסף", "member24@example.com", "AQAAAAIAAYagAAAAEAIUxhSUWDY2HfT0QzMuJ6SSPObE3qZD5b152/eDCrpqSrPvpLBHBHSnONPqdS9Gug==", "Member" },
                    { 25, new DateTime(2026, 1, 1, 8, 24, 0, 0, DateTimeKind.Utc), "בועז עמר", "member25@example.com", "AQAAAAIAAYagAAAAEHqw+++tAWL20y9zkOkWr0SDuW1BTv4ZBk2QxAggHi3VKajib30RGJk9g/s5CbPLZQ==", "Member" },
                    { 26, new DateTime(2026, 1, 1, 8, 25, 0, 0, DateTimeKind.Utc), "קרן שטרן", "member26@example.com", "AQAAAAIAAYagAAAAECxksVoqqAyb996nxYE2ltID3XsGXo41I75SrkyKN8BIxMYgALHweBtsUrCXV2jc/g==", "Member" },
                    { 27, new DateTime(2026, 1, 1, 8, 26, 0, 0, DateTimeKind.Utc), "תום ברק", "member27@example.com", "AQAAAAIAAYagAAAAENNnO6Pe6FaLSjQphp4OQuWGjxdCmscT/XSGsQrITV2vIgKAK6Mtr0WukiXxogoKOw==", "Member" },
                    { 28, new DateTime(2026, 1, 1, 8, 27, 0, 0, DateTimeKind.Utc), "ענבל נחום", "member28@example.com", "AQAAAAIAAYagAAAAEKkVy9YNrFFnef7SwjoI19U+5QGIoqHI2yOoeHrnieHANYrnMHtS1zC75QkXFTU9ig==", "Member" },
                    { 29, new DateTime(2026, 1, 1, 8, 28, 0, 0, DateTimeKind.Utc), "אריאל זילברמן", "member29@example.com", "AQAAAAIAAYagAAAAEElSskOd8DNJnMdkH60VsGCnkmmjBpCJdKpD2ao72ZCgxMnVGmv5WHF3BvYHP1CgRw==", "Member" },
                    { 30, new DateTime(2026, 1, 1, 8, 29, 0, 0, DateTimeKind.Utc), "סיון מלכה", "member30@example.com", "AQAAAAIAAYagAAAAEO6uK89XODgnYU0OryfgzS7QG9YFrEWXPv5d9ELY8+qngZsv8mTz24mjepPxBYJWAA==", "Member" }
                });

            migrationBuilder.InsertData(
                table: "TaskItems",
                columns: new[] { "Id", "CompletedAt", "CreatedAt", "Description", "ProjectId", "Status", "TakenByUserId", "Title" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 1, 3, 9, 0, 0, 0, DateTimeKind.Utc), "אפיון דרישות עבור הפרויקט אתר החברה", 1, "Open", null, "אפיון דרישות – אתר החברה" },
                    { 2, null, new DateTime(2026, 1, 3, 10, 0, 0, 0, DateTimeKind.Utc), "עיצוב מסכים עבור הפרויקט אתר החברה", 1, "Open", null, "עיצוב מסכים – אתר החברה" },
                    { 3, null, new DateTime(2026, 1, 3, 11, 0, 0, 0, DateTimeKind.Utc), "הקמת בסיס נתונים עבור הפרויקט אתר החברה", 1, "Open", null, "הקמת בסיס נתונים – אתר החברה" },
                    { 4, null, new DateTime(2026, 1, 3, 12, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד שרת עבור הפרויקט אתר החברה", 1, "Open", null, "פיתוח צד שרת – אתר החברה" },
                    { 5, null, new DateTime(2026, 1, 3, 13, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד לקוח עבור הפרויקט אתר החברה", 1, "Open", null, "פיתוח צד לקוח – אתר החברה" },
                    { 6, null, new DateTime(2026, 1, 3, 14, 0, 0, 0, DateTimeKind.Utc), "כתיבת בדיקות עבור הפרויקט אתר החברה", 1, "Open", null, "כתיבת בדיקות – אתר החברה" },
                    { 7, null, new DateTime(2026, 1, 3, 15, 0, 0, 0, DateTimeKind.Utc), "תיקון באגים עבור הפרויקט אתר החברה", 1, "Open", null, "תיקון באגים – אתר החברה" },
                    { 8, null, new DateTime(2026, 1, 3, 16, 0, 0, 0, DateTimeKind.Utc), "שיפור ביצועים עבור הפרויקט אתר החברה", 1, "Open", null, "שיפור ביצועים – אתר החברה" },
                    { 9, null, new DateTime(2026, 1, 3, 17, 0, 0, 0, DateTimeKind.Utc), "כתיבת תיעוד עבור הפרויקט אתר החברה", 1, "Open", null, "כתיבת תיעוד – אתר החברה" },
                    { 10, null, new DateTime(2026, 1, 3, 18, 0, 0, 0, DateTimeKind.Utc), "העלאה לאוויר עבור הפרויקט אתר החברה", 1, "Open", null, "העלאה לאוויר – אתר החברה" },
                    { 11, null, new DateTime(2026, 1, 3, 19, 0, 0, 0, DateTimeKind.Utc), "אפיון דרישות עבור הפרויקט אפליקציית מובייל", 2, "Open", null, "אפיון דרישות – אפליקציית מובייל" },
                    { 12, null, new DateTime(2026, 1, 3, 20, 0, 0, 0, DateTimeKind.Utc), "עיצוב מסכים עבור הפרויקט אפליקציית מובייל", 2, "Open", null, "עיצוב מסכים – אפליקציית מובייל" },
                    { 13, null, new DateTime(2026, 1, 3, 21, 0, 0, 0, DateTimeKind.Utc), "הקמת בסיס נתונים עבור הפרויקט אפליקציית מובייל", 2, "Open", null, "הקמת בסיס נתונים – אפליקציית מובייל" },
                    { 14, null, new DateTime(2026, 1, 3, 22, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד שרת עבור הפרויקט אפליקציית מובייל", 2, "Open", null, "פיתוח צד שרת – אפליקציית מובייל" },
                    { 15, null, new DateTime(2026, 1, 3, 23, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד לקוח עבור הפרויקט אפליקציית מובייל", 2, "Open", null, "פיתוח צד לקוח – אפליקציית מובייל" },
                    { 16, null, new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), "כתיבת בדיקות עבור הפרויקט אפליקציית מובייל", 2, "Open", null, "כתיבת בדיקות – אפליקציית מובייל" },
                    { 17, null, new DateTime(2026, 1, 4, 1, 0, 0, 0, DateTimeKind.Utc), "תיקון באגים עבור הפרויקט אפליקציית מובייל", 2, "Open", null, "תיקון באגים – אפליקציית מובייל" },
                    { 18, null, new DateTime(2026, 1, 4, 2, 0, 0, 0, DateTimeKind.Utc), "שיפור ביצועים עבור הפרויקט אפליקציית מובייל", 2, "Open", null, "שיפור ביצועים – אפליקציית מובייל" },
                    { 19, null, new DateTime(2026, 1, 4, 3, 0, 0, 0, DateTimeKind.Utc), "כתיבת תיעוד עבור הפרויקט אפליקציית מובייל", 2, "Open", null, "כתיבת תיעוד – אפליקציית מובייל" },
                    { 20, null, new DateTime(2026, 1, 4, 4, 0, 0, 0, DateTimeKind.Utc), "העלאה לאוויר עבור הפרויקט אפליקציית מובייל", 2, "Open", null, "העלאה לאוויר – אפליקציית מובייל" },
                    { 21, null, new DateTime(2026, 1, 4, 5, 0, 0, 0, DateTimeKind.Utc), "אפיון דרישות עבור הפרויקט מערכת CRM", 3, "Open", null, "אפיון דרישות – מערכת CRM" },
                    { 22, null, new DateTime(2026, 1, 4, 6, 0, 0, 0, DateTimeKind.Utc), "עיצוב מסכים עבור הפרויקט מערכת CRM", 3, "Open", null, "עיצוב מסכים – מערכת CRM" },
                    { 23, null, new DateTime(2026, 1, 4, 7, 0, 0, 0, DateTimeKind.Utc), "הקמת בסיס נתונים עבור הפרויקט מערכת CRM", 3, "Open", null, "הקמת בסיס נתונים – מערכת CRM" },
                    { 24, null, new DateTime(2026, 1, 4, 8, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד שרת עבור הפרויקט מערכת CRM", 3, "Open", null, "פיתוח צד שרת – מערכת CRM" },
                    { 25, null, new DateTime(2026, 1, 4, 9, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד לקוח עבור הפרויקט מערכת CRM", 3, "Open", null, "פיתוח צד לקוח – מערכת CRM" },
                    { 26, null, new DateTime(2026, 1, 4, 10, 0, 0, 0, DateTimeKind.Utc), "כתיבת בדיקות עבור הפרויקט מערכת CRM", 3, "Open", null, "כתיבת בדיקות – מערכת CRM" },
                    { 27, null, new DateTime(2026, 1, 4, 11, 0, 0, 0, DateTimeKind.Utc), "תיקון באגים עבור הפרויקט מערכת CRM", 3, "Open", null, "תיקון באגים – מערכת CRM" },
                    { 28, null, new DateTime(2026, 1, 4, 12, 0, 0, 0, DateTimeKind.Utc), "שיפור ביצועים עבור הפרויקט מערכת CRM", 3, "Open", null, "שיפור ביצועים – מערכת CRM" },
                    { 29, null, new DateTime(2026, 1, 4, 13, 0, 0, 0, DateTimeKind.Utc), "כתיבת תיעוד עבור הפרויקט מערכת CRM", 3, "Open", null, "כתיבת תיעוד – מערכת CRM" },
                    { 30, null, new DateTime(2026, 1, 4, 14, 0, 0, 0, DateTimeKind.Utc), "העלאה לאוויר עבור הפרויקט מערכת CRM", 3, "Open", null, "העלאה לאוויר – מערכת CRM" },
                    { 31, null, new DateTime(2026, 1, 4, 15, 0, 0, 0, DateTimeKind.Utc), "אפיון דרישות עבור הפרויקט חנות מקוונת", 4, "Open", null, "אפיון דרישות – חנות מקוונת" },
                    { 32, null, new DateTime(2026, 1, 4, 16, 0, 0, 0, DateTimeKind.Utc), "עיצוב מסכים עבור הפרויקט חנות מקוונת", 4, "Open", null, "עיצוב מסכים – חנות מקוונת" },
                    { 33, null, new DateTime(2026, 1, 4, 17, 0, 0, 0, DateTimeKind.Utc), "הקמת בסיס נתונים עבור הפרויקט חנות מקוונת", 4, "Open", null, "הקמת בסיס נתונים – חנות מקוונת" },
                    { 34, null, new DateTime(2026, 1, 4, 18, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד שרת עבור הפרויקט חנות מקוונת", 4, "Open", null, "פיתוח צד שרת – חנות מקוונת" },
                    { 35, null, new DateTime(2026, 1, 4, 19, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד לקוח עבור הפרויקט חנות מקוונת", 4, "Open", null, "פיתוח צד לקוח – חנות מקוונת" },
                    { 36, null, new DateTime(2026, 1, 4, 20, 0, 0, 0, DateTimeKind.Utc), "כתיבת בדיקות עבור הפרויקט חנות מקוונת", 4, "Open", null, "כתיבת בדיקות – חנות מקוונת" },
                    { 37, null, new DateTime(2026, 1, 4, 21, 0, 0, 0, DateTimeKind.Utc), "תיקון באגים עבור הפרויקט חנות מקוונת", 4, "Open", null, "תיקון באגים – חנות מקוונת" },
                    { 38, null, new DateTime(2026, 1, 4, 22, 0, 0, 0, DateTimeKind.Utc), "שיפור ביצועים עבור הפרויקט חנות מקוונת", 4, "Open", null, "שיפור ביצועים – חנות מקוונת" },
                    { 39, null, new DateTime(2026, 1, 4, 23, 0, 0, 0, DateTimeKind.Utc), "כתיבת תיעוד עבור הפרויקט חנות מקוונת", 4, "Open", null, "כתיבת תיעוד – חנות מקוונת" },
                    { 40, null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), "העלאה לאוויר עבור הפרויקט חנות מקוונת", 4, "Open", null, "העלאה לאוויר – חנות מקוונת" },
                    { 41, null, new DateTime(2026, 1, 5, 1, 0, 0, 0, DateTimeKind.Utc), "אפיון דרישות עבור הפרויקט מערכת שכר", 5, "Open", null, "אפיון דרישות – מערכת שכר" },
                    { 42, null, new DateTime(2026, 1, 5, 2, 0, 0, 0, DateTimeKind.Utc), "עיצוב מסכים עבור הפרויקט מערכת שכר", 5, "Open", null, "עיצוב מסכים – מערכת שכר" },
                    { 43, null, new DateTime(2026, 1, 5, 3, 0, 0, 0, DateTimeKind.Utc), "הקמת בסיס נתונים עבור הפרויקט מערכת שכר", 5, "Open", null, "הקמת בסיס נתונים – מערכת שכר" },
                    { 44, null, new DateTime(2026, 1, 5, 4, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד שרת עבור הפרויקט מערכת שכר", 5, "Open", null, "פיתוח צד שרת – מערכת שכר" },
                    { 45, null, new DateTime(2026, 1, 5, 5, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד לקוח עבור הפרויקט מערכת שכר", 5, "Open", null, "פיתוח צד לקוח – מערכת שכר" },
                    { 46, null, new DateTime(2026, 1, 5, 6, 0, 0, 0, DateTimeKind.Utc), "כתיבת בדיקות עבור הפרויקט מערכת שכר", 5, "Open", null, "כתיבת בדיקות – מערכת שכר" },
                    { 47, null, new DateTime(2026, 1, 5, 7, 0, 0, 0, DateTimeKind.Utc), "תיקון באגים עבור הפרויקט מערכת שכר", 5, "Open", null, "תיקון באגים – מערכת שכר" },
                    { 48, null, new DateTime(2026, 1, 5, 8, 0, 0, 0, DateTimeKind.Utc), "שיפור ביצועים עבור הפרויקט מערכת שכר", 5, "Open", null, "שיפור ביצועים – מערכת שכר" },
                    { 49, null, new DateTime(2026, 1, 5, 9, 0, 0, 0, DateTimeKind.Utc), "כתיבת תיעוד עבור הפרויקט מערכת שכר", 5, "Open", null, "כתיבת תיעוד – מערכת שכר" },
                    { 50, null, new DateTime(2026, 1, 5, 10, 0, 0, 0, DateTimeKind.Utc), "העלאה לאוויר עבור הפרויקט מערכת שכר", 5, "Open", null, "העלאה לאוויר – מערכת שכר" },
                    { 51, null, new DateTime(2026, 1, 5, 11, 0, 0, 0, DateTimeKind.Utc), "אפיון דרישות עבור הפרויקט פורטל עובדים", 6, "Open", null, "אפיון דרישות – פורטל עובדים" },
                    { 52, null, new DateTime(2026, 1, 5, 12, 0, 0, 0, DateTimeKind.Utc), "עיצוב מסכים עבור הפרויקט פורטל עובדים", 6, "Open", null, "עיצוב מסכים – פורטל עובדים" },
                    { 53, null, new DateTime(2026, 1, 5, 13, 0, 0, 0, DateTimeKind.Utc), "הקמת בסיס נתונים עבור הפרויקט פורטל עובדים", 6, "Open", null, "הקמת בסיס נתונים – פורטל עובדים" },
                    { 54, null, new DateTime(2026, 1, 5, 14, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד שרת עבור הפרויקט פורטל עובדים", 6, "Open", null, "פיתוח צד שרת – פורטל עובדים" },
                    { 55, null, new DateTime(2026, 1, 5, 15, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד לקוח עבור הפרויקט פורטל עובדים", 6, "Open", null, "פיתוח צד לקוח – פורטל עובדים" },
                    { 56, null, new DateTime(2026, 1, 5, 16, 0, 0, 0, DateTimeKind.Utc), "כתיבת בדיקות עבור הפרויקט פורטל עובדים", 6, "Open", null, "כתיבת בדיקות – פורטל עובדים" },
                    { 57, null, new DateTime(2026, 1, 5, 17, 0, 0, 0, DateTimeKind.Utc), "תיקון באגים עבור הפרויקט פורטל עובדים", 6, "Open", null, "תיקון באגים – פורטל עובדים" },
                    { 58, null, new DateTime(2026, 1, 5, 18, 0, 0, 0, DateTimeKind.Utc), "שיפור ביצועים עבור הפרויקט פורטל עובדים", 6, "Open", null, "שיפור ביצועים – פורטל עובדים" },
                    { 59, null, new DateTime(2026, 1, 5, 19, 0, 0, 0, DateTimeKind.Utc), "כתיבת תיעוד עבור הפרויקט פורטל עובדים", 6, "Open", null, "כתיבת תיעוד – פורטל עובדים" },
                    { 60, null, new DateTime(2026, 1, 5, 20, 0, 0, 0, DateTimeKind.Utc), "העלאה לאוויר עבור הפרויקט פורטל עובדים", 6, "Open", null, "העלאה לאוויר – פורטל עובדים" },
                    { 61, null, new DateTime(2026, 1, 5, 21, 0, 0, 0, DateTimeKind.Utc), "אפיון דרישות עבור הפרויקט מערכת מלאי", 7, "Open", null, "אפיון דרישות – מערכת מלאי" },
                    { 62, null, new DateTime(2026, 1, 5, 22, 0, 0, 0, DateTimeKind.Utc), "עיצוב מסכים עבור הפרויקט מערכת מלאי", 7, "Open", null, "עיצוב מסכים – מערכת מלאי" },
                    { 63, null, new DateTime(2026, 1, 5, 23, 0, 0, 0, DateTimeKind.Utc), "הקמת בסיס נתונים עבור הפרויקט מערכת מלאי", 7, "Open", null, "הקמת בסיס נתונים – מערכת מלאי" },
                    { 64, null, new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד שרת עבור הפרויקט מערכת מלאי", 7, "Open", null, "פיתוח צד שרת – מערכת מלאי" },
                    { 65, null, new DateTime(2026, 1, 6, 1, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד לקוח עבור הפרויקט מערכת מלאי", 7, "Open", null, "פיתוח צד לקוח – מערכת מלאי" },
                    { 66, null, new DateTime(2026, 1, 6, 2, 0, 0, 0, DateTimeKind.Utc), "כתיבת בדיקות עבור הפרויקט מערכת מלאי", 7, "Open", null, "כתיבת בדיקות – מערכת מלאי" },
                    { 67, null, new DateTime(2026, 1, 6, 3, 0, 0, 0, DateTimeKind.Utc), "תיקון באגים עבור הפרויקט מערכת מלאי", 7, "Open", null, "תיקון באגים – מערכת מלאי" },
                    { 68, null, new DateTime(2026, 1, 6, 4, 0, 0, 0, DateTimeKind.Utc), "שיפור ביצועים עבור הפרויקט מערכת מלאי", 7, "Open", null, "שיפור ביצועים – מערכת מלאי" },
                    { 69, null, new DateTime(2026, 1, 6, 5, 0, 0, 0, DateTimeKind.Utc), "כתיבת תיעוד עבור הפרויקט מערכת מלאי", 7, "Open", null, "כתיבת תיעוד – מערכת מלאי" },
                    { 70, null, new DateTime(2026, 1, 6, 6, 0, 0, 0, DateTimeKind.Utc), "העלאה לאוויר עבור הפרויקט מערכת מלאי", 7, "Open", null, "העלאה לאוויר – מערכת מלאי" },
                    { 71, null, new DateTime(2026, 1, 6, 7, 0, 0, 0, DateTimeKind.Utc), "אפיון דרישות עבור הפרויקט דוחות ניהול", 8, "Open", null, "אפיון דרישות – דוחות ניהול" },
                    { 72, null, new DateTime(2026, 1, 6, 8, 0, 0, 0, DateTimeKind.Utc), "עיצוב מסכים עבור הפרויקט דוחות ניהול", 8, "Open", null, "עיצוב מסכים – דוחות ניהול" },
                    { 73, null, new DateTime(2026, 1, 6, 9, 0, 0, 0, DateTimeKind.Utc), "הקמת בסיס נתונים עבור הפרויקט דוחות ניהול", 8, "Open", null, "הקמת בסיס נתונים – דוחות ניהול" },
                    { 74, null, new DateTime(2026, 1, 6, 10, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד שרת עבור הפרויקט דוחות ניהול", 8, "Open", null, "פיתוח צד שרת – דוחות ניהול" },
                    { 75, null, new DateTime(2026, 1, 6, 11, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד לקוח עבור הפרויקט דוחות ניהול", 8, "Open", null, "פיתוח צד לקוח – דוחות ניהול" },
                    { 76, null, new DateTime(2026, 1, 6, 12, 0, 0, 0, DateTimeKind.Utc), "כתיבת בדיקות עבור הפרויקט דוחות ניהול", 8, "Open", null, "כתיבת בדיקות – דוחות ניהול" },
                    { 77, null, new DateTime(2026, 1, 6, 13, 0, 0, 0, DateTimeKind.Utc), "תיקון באגים עבור הפרויקט דוחות ניהול", 8, "Open", null, "תיקון באגים – דוחות ניהול" },
                    { 78, null, new DateTime(2026, 1, 6, 14, 0, 0, 0, DateTimeKind.Utc), "שיפור ביצועים עבור הפרויקט דוחות ניהול", 8, "Open", null, "שיפור ביצועים – דוחות ניהול" },
                    { 79, null, new DateTime(2026, 1, 6, 15, 0, 0, 0, DateTimeKind.Utc), "כתיבת תיעוד עבור הפרויקט דוחות ניהול", 8, "Open", null, "כתיבת תיעוד – דוחות ניהול" },
                    { 80, null, new DateTime(2026, 1, 6, 16, 0, 0, 0, DateTimeKind.Utc), "העלאה לאוויר עבור הפרויקט דוחות ניהול", 8, "Open", null, "העלאה לאוויר – דוחות ניהול" },
                    { 81, null, new DateTime(2026, 1, 6, 17, 0, 0, 0, DateTimeKind.Utc), "אפיון דרישות עבור הפרויקט מערכת הזמנות", 9, "Open", null, "אפיון דרישות – מערכת הזמנות" },
                    { 82, null, new DateTime(2026, 1, 6, 18, 0, 0, 0, DateTimeKind.Utc), "עיצוב מסכים עבור הפרויקט מערכת הזמנות", 9, "Open", null, "עיצוב מסכים – מערכת הזמנות" },
                    { 83, null, new DateTime(2026, 1, 6, 19, 0, 0, 0, DateTimeKind.Utc), "הקמת בסיס נתונים עבור הפרויקט מערכת הזמנות", 9, "Open", null, "הקמת בסיס נתונים – מערכת הזמנות" },
                    { 84, null, new DateTime(2026, 1, 6, 20, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד שרת עבור הפרויקט מערכת הזמנות", 9, "Open", null, "פיתוח צד שרת – מערכת הזמנות" },
                    { 85, null, new DateTime(2026, 1, 6, 21, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד לקוח עבור הפרויקט מערכת הזמנות", 9, "Open", null, "פיתוח צד לקוח – מערכת הזמנות" },
                    { 86, null, new DateTime(2026, 1, 6, 22, 0, 0, 0, DateTimeKind.Utc), "כתיבת בדיקות עבור הפרויקט מערכת הזמנות", 9, "Open", null, "כתיבת בדיקות – מערכת הזמנות" },
                    { 87, null, new DateTime(2026, 1, 6, 23, 0, 0, 0, DateTimeKind.Utc), "תיקון באגים עבור הפרויקט מערכת הזמנות", 9, "Open", null, "תיקון באגים – מערכת הזמנות" },
                    { 88, null, new DateTime(2026, 1, 7, 0, 0, 0, 0, DateTimeKind.Utc), "שיפור ביצועים עבור הפרויקט מערכת הזמנות", 9, "Open", null, "שיפור ביצועים – מערכת הזמנות" },
                    { 89, null, new DateTime(2026, 1, 7, 1, 0, 0, 0, DateTimeKind.Utc), "כתיבת תיעוד עבור הפרויקט מערכת הזמנות", 9, "Open", null, "כתיבת תיעוד – מערכת הזמנות" },
                    { 90, null, new DateTime(2026, 1, 7, 2, 0, 0, 0, DateTimeKind.Utc), "העלאה לאוויר עבור הפרויקט מערכת הזמנות", 9, "Open", null, "העלאה לאוויר – מערכת הזמנות" },
                    { 91, null, new DateTime(2026, 1, 7, 3, 0, 0, 0, DateTimeKind.Utc), "אפיון דרישות עבור הפרויקט תשתית ענן", 10, "Open", null, "אפיון דרישות – תשתית ענן" },
                    { 92, null, new DateTime(2026, 1, 7, 4, 0, 0, 0, DateTimeKind.Utc), "עיצוב מסכים עבור הפרויקט תשתית ענן", 10, "Open", null, "עיצוב מסכים – תשתית ענן" },
                    { 93, null, new DateTime(2026, 1, 7, 5, 0, 0, 0, DateTimeKind.Utc), "הקמת בסיס נתונים עבור הפרויקט תשתית ענן", 10, "Open", null, "הקמת בסיס נתונים – תשתית ענן" },
                    { 94, null, new DateTime(2026, 1, 7, 6, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד שרת עבור הפרויקט תשתית ענן", 10, "Open", null, "פיתוח צד שרת – תשתית ענן" },
                    { 95, null, new DateTime(2026, 1, 7, 7, 0, 0, 0, DateTimeKind.Utc), "פיתוח צד לקוח עבור הפרויקט תשתית ענן", 10, "Open", null, "פיתוח צד לקוח – תשתית ענן" },
                    { 96, null, new DateTime(2026, 1, 7, 8, 0, 0, 0, DateTimeKind.Utc), "כתיבת בדיקות עבור הפרויקט תשתית ענן", 10, "Open", null, "כתיבת בדיקות – תשתית ענן" },
                    { 97, null, new DateTime(2026, 1, 7, 9, 0, 0, 0, DateTimeKind.Utc), "תיקון באגים עבור הפרויקט תשתית ענן", 10, "Open", null, "תיקון באגים – תשתית ענן" },
                    { 98, null, new DateTime(2026, 1, 7, 10, 0, 0, 0, DateTimeKind.Utc), "שיפור ביצועים עבור הפרויקט תשתית ענן", 10, "Open", null, "שיפור ביצועים – תשתית ענן" },
                    { 99, null, new DateTime(2026, 1, 7, 11, 0, 0, 0, DateTimeKind.Utc), "כתיבת תיעוד עבור הפרויקט תשתית ענן", 10, "Open", null, "כתיבת תיעוד – תשתית ענן" },
                    { 100, null, new DateTime(2026, 1, 7, 12, 0, 0, 0, DateTimeKind.Utc), "העלאה לאוויר עבור הפרויקט תשתית ענן", 10, "Open", null, "העלאה לאוויר – תשתית ענן" }
                });

            migrationBuilder.InsertData(
                table: "TaskItemLabel",
                columns: new[] { "LabelId", "TaskItemId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 4, 4 },
                    { 5, 5 },
                    { 9, 5 },
                    { 7, 7 },
                    { 8, 8 },
                    { 4, 10 },
                    { 10, 10 },
                    { 1, 11 },
                    { 3, 13 },
                    { 4, 14 },
                    { 6, 16 },
                    { 7, 17 },
                    { 9, 19 },
                    { 4, 20 },
                    { 10, 20 },
                    { 2, 22 },
                    { 3, 23 },
                    { 5, 25 },
                    { 9, 25 },
                    { 6, 26 },
                    { 8, 28 },
                    { 9, 29 },
                    { 1, 31 },
                    { 2, 32 },
                    { 4, 34 },
                    { 5, 35 },
                    { 9, 35 },
                    { 7, 37 },
                    { 8, 38 },
                    { 4, 40 },
                    { 10, 40 },
                    { 1, 41 },
                    { 3, 43 },
                    { 4, 44 },
                    { 6, 46 },
                    { 7, 47 },
                    { 9, 49 },
                    { 4, 50 },
                    { 10, 50 },
                    { 2, 52 },
                    { 3, 53 },
                    { 5, 55 },
                    { 9, 55 },
                    { 6, 56 },
                    { 8, 58 },
                    { 9, 59 },
                    { 1, 61 },
                    { 2, 62 },
                    { 4, 64 },
                    { 5, 65 },
                    { 9, 65 },
                    { 7, 67 },
                    { 8, 68 },
                    { 4, 70 },
                    { 10, 70 },
                    { 1, 71 },
                    { 3, 73 },
                    { 4, 74 },
                    { 6, 76 },
                    { 7, 77 },
                    { 9, 79 },
                    { 4, 80 },
                    { 10, 80 },
                    { 2, 82 },
                    { 3, 83 },
                    { 5, 85 },
                    { 9, 85 },
                    { 6, 86 },
                    { 8, 88 },
                    { 9, 89 },
                    { 1, 91 },
                    { 2, 92 },
                    { 4, 94 },
                    { 5, 95 },
                    { 9, 95 },
                    { 7, 97 },
                    { 8, 98 },
                    { 4, 100 },
                    { 10, 100 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 5, 5 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 9, 5 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 7, 7 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 8, 8 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 4, 10 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 10, 10 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 1, 11 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 3, 13 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 4, 14 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 6, 16 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 7, 17 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 9, 19 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 4, 20 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 10, 20 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 2, 22 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 3, 23 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 5, 25 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 9, 25 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 6, 26 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 8, 28 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 9, 29 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 1, 31 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 2, 32 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 4, 34 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 5, 35 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 9, 35 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 7, 37 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 8, 38 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 4, 40 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 10, 40 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 1, 41 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 3, 43 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 4, 44 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 6, 46 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 7, 47 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 9, 49 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 4, 50 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 10, 50 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 2, 52 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 3, 53 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 5, 55 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 9, 55 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 6, 56 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 8, 58 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 9, 59 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 1, 61 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 2, 62 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 4, 64 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 5, 65 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 9, 65 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 7, 67 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 8, 68 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 4, 70 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 10, 70 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 1, 71 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 3, 73 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 4, 74 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 6, 76 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 7, 77 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 9, 79 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 4, 80 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 10, 80 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 2, 82 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 3, 83 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 5, 85 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 9, 85 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 6, 86 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 8, 88 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 9, 89 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 1, 91 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 2, 92 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 4, 94 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 5, 95 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 9, 95 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 7, 97 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 8, 98 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 4, 100 });

            migrationBuilder.DeleteData(
                table: "TaskItemLabel",
                keyColumns: new[] { "LabelId", "TaskItemId" },
                keyValues: new object[] { 10, 100 });

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
