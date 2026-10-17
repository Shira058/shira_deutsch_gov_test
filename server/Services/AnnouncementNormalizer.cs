using System.Globalization;
using Microsoft.Extensions.Options;
using RA_MuniAnnouncements.DTOs;
using RA_MuniAnnouncements.Models;
using RA_MuniAnnouncements.Options;
using RA_MuniAnnouncements.Services.Interfaces;

namespace RA_MuniAnnouncements.Services;

public sealed class AnnouncementNormalizer(IHtmlTextExtractor htmlTextExtractor, IOptions<NormalizationOptions> options)
    : IAnnouncementNormalizer
{
    private const string PublishedStatus = "Published";
    private const int SummaryMaxLength = 150;
    private const string DateFormat = "dd/MM/yyyy";

    // Residents read dates in local time: 22:30Z is already the next day in Israel.
    private static readonly TimeZoneInfo IsraelTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jerusalem");

    public AnnouncementsResponseDto Normalize(AuthoritySiteSettings settings, IEnumerable<CmsAnnouncement> raw, DateTimeOffset now)
    {
        var enabledCategories = settings.EnabledCategories.ToHashSet();
        var urgentCategories = settings.UrgentCategories.ToHashSet();
        var maxItems = settings.MaxItems is int max and > 0 ? max : options.Value.DefaultMaxItems;

        var announcements = raw
            // Re-checking the authority guards against a CMS that returns another tenant's content.
            .Where(a => a.AuthorityCode == settings.AuthorityCode
                        && a.Status == PublishedStatus
                        && a.PublishDate <= now && now < a.ExpiryDate
                        && enabledCategories.Contains(a.Category))
            .OrderByDescending(a => urgentCategories.Contains(a.Category))
            .ThenByDescending(a => a.PublishDate)
            // Same-date items need a stable order, or the list could shuffle between requests.
            .ThenBy(a => a.Id, StringComparer.Ordinal)
            .Take(maxItems)
            // Mapped after Take, so HTML is parsed only for items that are actually returned.
            .Select(a => new AnnouncementDto
            {
                Id = a.Id,
                Title = a.Title,
                Summary = htmlTextExtractor.ToPlainSummary(a.Body, SummaryMaxLength),
                CategoryName = settings.CategoryDisplayNames[a.Category],
                IsUrgent = urgentCategories.Contains(a.Category),
                PublishDate = TimeZoneInfo.ConvertTime(a.PublishDate, IsraelTimeZone)
                    .ToString(DateFormat, CultureInfo.InvariantCulture),
            })
            .ToList();

        return new AnnouncementsResponseDto
        {
            AuthorityCode = settings.AuthorityCode,
            AuthorityName = settings.AuthorityName,
            Announcements = announcements,
        };
    }
}
