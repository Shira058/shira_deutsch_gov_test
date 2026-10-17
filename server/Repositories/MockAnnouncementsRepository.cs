using RA_MuniAnnouncements.Models;
using RA_MuniAnnouncements.Repositories.Interfaces;

namespace RA_MuniAnnouncements.Repositories;

/// <summary>
/// Hard-coded CMS data covering every normalization edge case.
/// Dates are relative to <see cref="TimeProvider"/> so the demo data never expires.
/// </summary>
public sealed class MockAnnouncementsRepository(TimeProvider timeProvider) : IAnnouncementsRepository
{
    public Task<IReadOnlyList<CmsAnnouncement>> GetAnnouncementsAsync(string authorityCode, CancellationToken ct)
    {
        // Deliberately returns every authority's items, so the normalizer's tenant check is always exercised.
        return Task.FromResult(Build(timeProvider.GetUtcNow()));
    }

    private static IReadOnlyList<CmsAnnouncement> Build(DateTimeOffset now)
    {
        var day = TimeSpan.FromDays(1);

        CmsAnnouncement Item(string id, string authorityCode, string category, string title, string body,
            TimeSpan publishedAgo, TimeSpan expiresIn, string status = "Published") => new()
        {
            Id = id,
            AuthorityCode = authorityCode,
            Category = category,
            Title = title,
            Body = body,
            Status = status,
            PublishDate = now - publishedAgo,
            ExpiryDate = now + expiresIn,
        };

        return
        [
            // Authority 6500: 8 active items, maxItems 5.
            Item("a-1001", "6500", "roads", "סגירת רחוב הרברט סמואל",
                "<p>בשל עבודות תשתית ייסגר הרחוב לתנועה בין השעות 08:00 ל-17:00.</p>", 2 * day, 5 * day),
            Item("a-1002", "6500", "water", "הפסקת מים בשכונת בית אליעזר",
                "<p>עקב תיקון צנרת תופסק אספקת המים <strong>מחר בין 09:00 ל-13:00</strong>.</p>", 3 * day, 2 * day),
            Item("a-1003", "6500", "events", "פסטיבל האביב בפארק חדרה",
                """
                <h2>פסטיבל&nbsp;האביב</h2><p>הופעות, דוכני אוכל &amp; סדנאות לכל המשפחה.</p>
                <ul><li>הכניסה חופשית</li><li>חניה בחניון העירוני</li></ul>
                <p>הפסטיבל יתקיים בפארק חדרה לאורך שלושה ימים, עם במה מרכזית, מתחם ילדים, שוק איכרים
                ומופע זיקוקים בערב הסיום. מומלץ להגיע בתחבורה ציבורית.</p>
                """, 1 * day, 10 * day),
            Item("a-1009", "6500", "roads", "שיפוץ מדרכות ברחוב הנשיא",
                "<p>עבודות שיפוץ המדרכות יימשכו כשבועיים.</p>", 4 * day, 3 * day),
            Item("a-1010", "6500", "events", "ערב קריאה בספרייה העירונית",
                "<p>ערב קריאה פתוח לקהל הרחב.</p>", 5 * day, 7 * day),
            Item("a-1011", "6500", "water", "שטיפת קווי מים",
                "<p>ייתכנו שינויים בלחץ המים במהלך השבוע.</p>", 6 * day, 1 * day),
            Item("a-1012", "6500", "roads", "הסדרי תנועה חדשים בכיכר הרכבת",
                "<p>שימו לב לשילוט החדש.</p>", 7 * day, 14 * day),
            // XSS payloads in the body (script, onerror, javascript: link) must reach the client as plain text only.
            Item("a-1013", "6500", "events", "הודעה עם תוכן זדוני",
                """<p>ההרשמה לחוגי הנוער לשנה החדשה נפתחה.</p><script>alert('body')</script><img src=x onerror="alert('img')"><a href="javascript:alert('link')">לפרטים והרשמה</a>""",
                TimeSpan.FromHours(1), 3 * day),

            // Authority 6500: items that must not be shown.
            Item("a-1004", "6500", "roads", "טיוטה: עבודות בכביש 4",
                "<p>טיוטה</p>", 1 * day, 5 * day, status: "Draft"),
            Item("a-1005", "6500", "events", "אירוע שהסתיים",
                "<p>פג תוקף אתמול</p>", 10 * day, -1 * day),
            Item("a-1006", "6500", "roads", "הודעה עתידית",
                "<p>תפורסם מחר</p>", -1 * day, 8 * day),
            Item("a-1007", "6500", "tenders", "מכרז לניקיון רחובות",
                "<p>קטגוריה שאינה מופעלת ברשות 6500</p>", 1 * day, 5 * day),

            // Authority 5000: different settings (tenders enabled, roads urgent, maxItems 3).
            // Shown for 5000 only: a 6500 request also receives it (see above) and the tenant check must drop it.
            Item("a-1008", "5000", "water", "הפסקת מים ברחוב דיזנגוף",
                "<p>עקב עבודות תחזוקה תופסק אספקת המים מחר בין 10:00 ל-14:00.</p>", 1 * day, 5 * day),
            Item("a-2001", "5000", "tenders", "מכרז לתאורת רחוב",
                "<p>ההצעות יוגשו עד סוף החודש.</p>", 2 * day, 20 * day),
            Item("a-2002", "5000", "roads", "סגירת נתיבי איילון",
                "<p>עבודות לילה בנתיבי איילון.</p>", 3 * day, 2 * day),
            Item("a-2003", "5000", "events", "מרתון תל אביב",
                "<p>קטגוריה שאינה מופעלת ברשות 5000.</p>", 1 * day, 5 * day),
            Item("a-2004", "5000", "water", "בדיקת איכות מים",
                "<p>תוצאות הבדיקה תקינות.</p>", 5 * day, 5 * day),

            // Authority 7000: has settings but no active items.
            Item("a-3001", "7000", "roads", "עבודות שהסתיימו",
                "<p>פג תוקף</p>", 30 * day, -2 * day),
        ];
    }
}
