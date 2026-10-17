using RA_MuniAnnouncements.DTOs;
using RA_MuniAnnouncements.Models;

namespace RA_MuniAnnouncements.Services.Interfaces;

/// <summary>Turns raw CMS announcements into the display model, according to the authority's settings.</summary>
public interface IAnnouncementNormalizer
{
    /// <summary>
    /// Filters to active announcements of enabled categories, maps display names and urgency,
    /// sorts urgent-first then newest-first, and limits to the authority's max items.
    /// </summary>
    /// <param name="settings">The settings of the authority being served.</param>
    /// <param name="raw">Raw announcements from the CMS.</param>
    /// <param name="now">The current time. Passed in so the normalization stays a pure, testable function.</param>
    AnnouncementsResponseDto Normalize(AuthoritySiteSettings settings, IEnumerable<CmsAnnouncement> raw, DateTimeOffset now);
}
