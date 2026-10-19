using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;

namespace TaskManager.Data.Seed;

// GN-05: loaded by the SeedData migration (GN-05-R3, DP-04-R3).
// HasData needs fixed values, so ids, dates and password hashes are constants.
internal static class SeedData
{
    private static readonly DateTime BaseDate = new(2026, 1, 1, 8, 0, 0, DateTimeKind.Utc);

    // GN-05-R1: ASP.NET Core PasswordHasher hashes of Demo1234!1 .. Demo1234!30, in user Id order
    private static readonly string[] PasswordHashes =
    [
        "AQAAAAIAAYagAAAAEE5k5ddjrJetqI1M2qMV43Wk4vJDwu/RKylBc4ibPQmou8GEspyUaQPnl4/YkPYmSw==",
        "AQAAAAIAAYagAAAAEBHsijZrUYxG3pey5UOPMPESjxtVleFPJgljOaHJoY2StfjQYAzhnkILH8aavu1sEQ==",
        "AQAAAAIAAYagAAAAEF5LUmT4pouBsnlQlSP/865LPeU2WKgzIO7JHbGVWfGmqyd+wpjBpjMduZevEmlWjg==",
        "AQAAAAIAAYagAAAAEC3JMflVxJFleAfDNJgSEMoopxrZLOdsZWEf+M4wV3iVPWyiysjBfiqtxf0Md8yhGA==",
        "AQAAAAIAAYagAAAAEDa9w1TCSQ2AWn2MFaWapxQTRUngo4q6Y4lqcYnso2QquhZKWMZEp9k4nJy9YFPe7Q==",
        "AQAAAAIAAYagAAAAEPyybscE1G9LPfCR5ttVL8pEl3LvvWgzj9CKLdyjWW3pwOsnRoA1cp+auipPoY9+5w==",
        "AQAAAAIAAYagAAAAEJm2QmwN58wi+wV+P2xXQ94DfwLXkoWAFDtBqFuwVgGRvGwPVyFFoG6zO2H13P7EOg==",
        "AQAAAAIAAYagAAAAEM0MOy/qQWruNEYi7pd1CxXIjnWP8GtO5hcvfn4yzWmRtWW9g4MTmiJjhO97IF1y0g==",
        "AQAAAAIAAYagAAAAEN8gkrDWynoYgq5g3N7dZdhHQSFrH5HMYHreuYj5wCD+VygXgcvzgtZOlH0dfcgfeA==",
        "AQAAAAIAAYagAAAAEBiP8dHXlSn7NLQapVYGyzfrxvgEnbY4mE1d2PIWQO10JrTL3sDhHu6ZzxcJ1Tlciw==",
        "AQAAAAIAAYagAAAAEKzjBuYl5A33dbnI6oXK+X5gyLAeq3uZifuNkMqbij6TyRZAaCyKRPWSSrh5+ZQRyw==",
        "AQAAAAIAAYagAAAAEBVp3k27qTmc/K1MaVF7H3p+EsYa19LgMw2v3Jb58JoSBIcOb9Qk5/x4c8ZIpyG80w==",
        "AQAAAAIAAYagAAAAEAW1tPIltG+UtP3ocTxoaQlkygREI8Ia0EiQdQ7KGxozThcENXaSqXvmZ+O+M7gpvQ==",
        "AQAAAAIAAYagAAAAEJIyu8oEcObM9NXqImUph4SRHVhvklKCCZHZ7VGgyOU55pLPs/URMEqS5qQ37nx8ZQ==",
        "AQAAAAIAAYagAAAAEDcJoFA1JamAT+H5xqn04tpenxT+Dw0RIZ58FLn9LOwfG9onIy5cgJnocKr4sDoI4A==",
        "AQAAAAIAAYagAAAAEOdW5vyqolh8JD2GCxKo7/aOalRshF5FKJcy1Jufy+JbzjcohFRo2hhlPp23jEkMCg==",
        "AQAAAAIAAYagAAAAEAOa7OWYfm6acCAPmB3HGP3dbXSYvqIU0GXPyfhtvjyUJktv/AvIXU4XQNohU/e79g==",
        "AQAAAAIAAYagAAAAEOmcjb88bBkoMeWnxbj2kYN44gm4K2N99fyE5G+a/TlE0/1+CFghDAZ0ba9ENlkdOA==",
        "AQAAAAIAAYagAAAAEPXrh0HMH+dQ6JmfTIUsMD259Nw0mvRdyIRNSDER5wYYKCrcHxc82SEqrxtpjFVNIw==",
        "AQAAAAIAAYagAAAAEC5iwDlgBk1yqbP64a30e0aFZkmh2BQykJsEYw4yaDeaXZfpvN2eISVOnTpwWOVRXw==",
        "AQAAAAIAAYagAAAAEDGtNyX6UcUTqyoxSIVvTZN7PfabJY14RCswv329Mf4Q79czgx67ouSgYt+bdXwcbw==",
        "AQAAAAIAAYagAAAAEILPYwH92N4yh29FriguXtLeoOHOE9PLjB/5QyZaSTP54N4vEcj+fIFajD2fs2/2bg==",
        "AQAAAAIAAYagAAAAEBC5wyg8e5cJNT/Fx1v4AI37wopFBUHwiRpkOe0pZ16ekZ9jiV91yvU01J/RHhXyYQ==",
        "AQAAAAIAAYagAAAAEAIUxhSUWDY2HfT0QzMuJ6SSPObE3qZD5b152/eDCrpqSrPvpLBHBHSnONPqdS9Gug==",
        "AQAAAAIAAYagAAAAEHqw+++tAWL20y9zkOkWr0SDuW1BTv4ZBk2QxAggHi3VKajib30RGJk9g/s5CbPLZQ==",
        "AQAAAAIAAYagAAAAECxksVoqqAyb996nxYE2ltID3XsGXo41I75SrkyKN8BIxMYgALHweBtsUrCXV2jc/g==",
        "AQAAAAIAAYagAAAAENNnO6Pe6FaLSjQphp4OQuWGjxdCmscT/XSGsQrITV2vIgKAK6Mtr0WukiXxogoKOw==",
        "AQAAAAIAAYagAAAAEKkVy9YNrFFnef7SwjoI19U+5QGIoqHI2yOoeHrnieHANYrnMHtS1zC75QkXFTU9ig==",
        "AQAAAAIAAYagAAAAEElSskOd8DNJnMdkH60VsGCnkmmjBpCJdKpD2ao72ZCgxMnVGmv5WHF3BvYHP1CgRw==",
        "AQAAAAIAAYagAAAAEO6uK89XODgnYU0OryfgzS7QG9YFrEWXPv5d9ELY8+qngZsv8mTz24mjepPxBYJWAA==",
    ];

    // Index 0 is the Admin; members are 2..30
    private static readonly string[] DisplayNames =
    [
        "מנהל המערכת",
        "דנה כהן", "יוסי לוי", "מיכל אברהם", "אבי מזרחי", "נועה פרץ", "רון ביטון", "שירה דהן",
        "עומר אזולאי", "תמר פרידמן", "איתי שפירא", "יעל גולן", "גיא אוחיון", "ליאת חדד", "אלון סויסה",
        "מאיה רוזן", "דוד קליין", "רותם בן דוד", "אורי וקנין", "הילה אשכנזי", "ניר גבאי",
        "שני טל", "עידו כץ", "אסנת יוסף", "בועז עמר", "קרן שטרן", "תום ברק", "ענבל נחום",
        "אריאל זילברמן", "סיון מלכה",
    ];

    private static readonly (string Name, string Description)[] Projects =
    [
        ("אתר החברה", "בניית אתר תדמית חדש לחברה"),
        ("אפליקציית מובייל", "אפליקציה לאנדרואיד ול-iOS עבור לקוחות"),
        ("מערכת CRM", "ניהול קשרי לקוחות ותהליכי מכירה"),
        ("חנות מקוונת", "חנות אונליין עם תשלום מאובטח"),
        ("מערכת שכר", "חישוב שכר והפקת דוחות לעובדים"),
        ("פורטל עובדים", "פורטל פנימי למידע, הודעות וטפסים"),
        ("מערכת מלאי", "ניהול מלאי, ספקים ומחסנים"),
        ("דוחות ניהול", "לוחות מחוונים ודוחות להנהלה"),
        ("מערכת הזמנות", "הזמנת תורים ושירותים אונליין"),
        ("תשתית ענן", "העברת השרתים לענן והקמת ניטור"),
    ];

    private static readonly string[] Labels =
        ["באג", "פיצ'ר", "דחוף", "עיצוב", "שרת", "לקוח", "בדיקות", "תיעוד", "ביצועים", "אבטחה"];

    // The 10 tasks of every project
    private static readonly string[] TaskTitles =
    [
        "אפיון דרישות", "עיצוב מסכים", "הקמת בסיס נתונים", "פיתוח צד שרת", "פיתוח צד לקוח",
        "כתיבת בדיקות", "תיקון באגים", "שיפור ביצועים", "כתיבת תיעוד", "העלאה לאוויר",
    ];

    public static void Seed(this ModelBuilder modelBuilder)
    {
        // GN-05-R4: user 1 is the only Admin
        modelBuilder.Entity<User>().HasData(DisplayNames.Select((name, i) => new User
        {
            Id = i + 1,
            Email = i == 0 ? "admin@example.com" : $"member{i + 1:00}@example.com",
            DisplayName = name,
            PasswordHash = PasswordHashes[i],
            Role = i == 0 ? Role.Admin : Role.Member,
            CreatedAt = BaseDate.AddMinutes(i),
        }));

        modelBuilder.Entity<Project>().HasData(Projects.Select((p, i) => new Project
        {
            Id = i + 1,
            Name = p.Name,
            Description = p.Description,
            CreatedAt = BaseDate.AddDays(1).AddHours(i),
        }));

        modelBuilder.Entity<Label>().HasData(Labels.Select((name, i) => new Label { Id = i + 1, Name = name }));

        // 10 Open tasks per project, created one hour apart
        modelBuilder.Entity<TaskItem>().HasData(Enumerable.Range(1, 100).Select(id =>
        {
            var projectIndex = (id - 1) / 10;
            var title = TaskTitles[(id - 1) % 10];
            return new TaskItem
            {
                Id = id,
                Title = $"{title} – {Projects[projectIndex].Name}",
                Description = $"{title} עבור הפרויקט {Projects[projectIndex].Name}",
                Status = Status.Open,
                ProjectId = projectIndex + 1,
                CreatedAt = BaseDate.AddDays(2).AddHours(id),
            };
        }));

        // About two thirds of the tasks get one or two labels; every third task has none
        modelBuilder.SharedTypeEntity<Dictionary<string, object>>("TaskItemLabel").HasData(
            Enumerable.Range(1, 100)
                .Where(id => id % 3 != 0)
                .SelectMany(id => LabelIdsFor(id).Select(labelId => (object)new { TaskItemId = id, LabelId = labelId }))
                .ToArray());
    }

    // One label, or two different labels for every fifth task
    private static int[] LabelIdsFor(int taskId) =>
        taskId % 5 == 0
            ? [(taskId - 1) % 10 + 1, (taskId + 3) % 10 + 1]
            : [(taskId - 1) % 10 + 1];
}
